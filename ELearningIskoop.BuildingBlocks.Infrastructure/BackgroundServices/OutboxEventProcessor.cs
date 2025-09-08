using ELearningIskoop.Shared.Infrastructure.Outbox;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Infrastructure.BackgroundServices
{
    public class OutboxEventProcessor : BackgroundService
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutboxEventProcessor> _logger;
        private readonly OutboxProcessorOptions _options;
        private DateTime _lastCleanupTime = DateTime.MinValue;

        public OutboxEventProcessor(
            IServiceProvider serviceProvider,
            ILogger<OutboxEventProcessor> logger,
            IOptions<OutboxProcessorOptions> options)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? new OutboxProcessorOptions();
        }



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Shared Outbox Event Processor started with following settings:");
            _logger.LogInformation("- Processing Interval: {Interval}s", _options.ProcessingIntervalSeconds);
            _logger.LogInformation("- Batch Size: {BatchSize}", _options.BatchSize);
            _logger.LogInformation("- Max Retry Attempts: {MaxRetries}", _options.MaxRetryAttempts);
            _logger.LogInformation("- Keep Processed Events: {KeepDays} days", _options.KeepProcessedEventsDays);
            _logger.LogInformation("- Auto Cleanup: {AutoCleanup}", _options.EnableAutoCleanup);
            _logger.LogInformation("- Priority Processing: {PriorityProcessing}", _options.PriorityProcessing);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_options.EnableProcessing)
                    {
                        await ProcessOutboxEvents(stoppingToken);
                    }
                    else
                    {
                        _logger.LogDebug("Outbox processing is disabled via configuration");
                    }

                    // Auto cleanup kontrolü
                    if (_options.EnableAutoCleanup && ShouldRunCleanup())
                    {
                        await CleanupOldEvents();
                        _lastCleanupTime = DateTime.UtcNow;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Shared Outbox Event Processor");
                }

                await Task.Delay(TimeSpan.FromSeconds(_options.ProcessingIntervalSeconds), stoppingToken);
            }

            _logger.LogInformation("Shared Outbox Event Processor stopped");
        }

        private async Task ProcessOutboxEvents(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var stopwatch = Stopwatch.StartNew();
            var maxProcessingTime = TimeSpan.FromMinutes(_options.MaxProcessingTimeMinutes);

            var unprocessedEvents = await outboxRepository.GetUnprocessedEventsAsync(_options.BatchSize);

            if (unprocessedEvents.Count == 0) return;

            _logger.LogInformation("Processing {EventCount} outbox events", unprocessedEvents.Count);

            var processedCount = 0;
            var failedCount = 0;

            foreach (var outboxEvent in unprocessedEvents)
            {
                // Timeout kontrolü
                if (stopwatch.Elapsed > maxProcessingTime)
                {
                    _logger.LogWarning("Processing timeout reached ({MaxTime}min), stopping batch processing",
                        _options.MaxProcessingTimeMinutes);
                    break;
                }

                try
                {
                    _logger.LogDebug("Processing outbox event {EventId} of type {EventType} from module {Module}",
                        outboxEvent.Id, GetEventTypeName(outboxEvent.EventType), outboxEvent.ModuleName);

                    // Event type'ı resolve et
                    var eventType = ResolveEventType(outboxEvent.EventType);
                    if (eventType == null)
                    {
                        _logger.LogWarning("Cannot find event type: {EventType} for event {EventId}",
                            outboxEvent.EventType, outboxEvent.Id);
                        await outboxRepository.MarkAsFailedAsync(outboxEvent.Id,
                            $"Event type not found: {outboxEvent.EventType}");
                        failedCount++;
                        continue;
                    }

                    // Event'i deserialize et
                    var domainEvent = JsonSerializer.Deserialize(outboxEvent.EventData, eventType, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    // MediatR ile event handler'ları çalıştır
                    await mediator.Publish(domainEvent, cancellationToken);

                    // Başarılı olarak işaretle
                    await outboxRepository.MarkAsProcessedAsync(outboxEvent.Id);
                    processedCount++;

                    _logger.LogDebug("Successfully processed outbox event {EventId}", outboxEvent.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process outbox event {EventId} from module {Module}. Correlation: {CorrelationId}",
                        outboxEvent.Id, outboxEvent.ModuleName, outboxEvent.CorrelationId);

                    await outboxRepository.MarkAsFailedAsync(outboxEvent.Id, ex.Message);
                    failedCount++;
                }
            }

            stopwatch.Stop();
            _logger.LogInformation("Completed processing batch: {ProcessedCount} successful, {FailedCount} failed, took {Duration}ms",
                processedCount, failedCount, stopwatch.ElapsedMilliseconds);
        }

        private Type ResolveEventType(string eventTypeName)
        {
            try
            {
                // Önce tam qualified name ile dene
                var eventType = Type.GetType(eventTypeName);
                if (eventType != null) return eventType;

                // Loaded assembly'lerde ara
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    eventType = assembly.GetType(eventTypeName);
                    if (eventType != null) return eventType;
                }

                // Son çare: assembly qualified name'i parse et
                var typeName = eventTypeName.Split(',')[0].Trim();
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    eventType = assembly.GetType(typeName);
                    if (eventType != null) return eventType;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving event type: {EventType}", eventTypeName);
                return null;
            }
        }

        private string GetEventTypeName(string fullTypeName)
        {
            try
            {
                var parts = fullTypeName.Split(',');
                var namespaceParts = parts[0].Split('.');
                return namespaceParts[^1]; // Son kısmı (class name)
            }
            catch
            {
                return fullTypeName;
            }
        }

        private bool ShouldRunCleanup()
        {
            return _lastCleanupTime == DateTime.MinValue ||
                   DateTime.UtcNow - _lastCleanupTime > TimeSpan.FromHours(_options.CleanupIntervalHours);
        }

        private async Task CleanupOldEvents()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();

                var cutoffDate = DateTime.UtcNow.AddDays(-_options.KeepProcessedEventsDays);
                await outboxRepository.DeleteProcessedEventsOlderThanAsync(cutoffDate);

                _logger.LogInformation("Cleaned up processed outbox events older than {CutoffDate}", cutoffDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cleanup old outbox events");
            }
        }
    }
}
