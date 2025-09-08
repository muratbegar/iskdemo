using ELearningIskoop.Shared.Infrastructure.Outbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Infrastructure.Outbox
{
    public class OutboxService : IOutboxService
    {
        private readonly OutboxDbContext _outboxContext;

        public OutboxService(OutboxDbContext outboxContext)
        {
            _outboxContext = outboxContext ?? throw new ArgumentNullException(nameof(outboxContext));
        }
        public async Task PublishAsync<T>(T domainEvent, string aggregateId, string aggregateType, string moduleName,
            string userId = null,
            string correlationId = null, int priority = 0) where T : class
        {

            var outboxEvent = new OutboxEvent
            {
                EventType = typeof(T).AssemblyQualifiedName,
                EventData = JsonSerializer.Serialize(domainEvent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                }),
                CreatedAt = DateTime.UtcNow,
                Processed = false,
                RetryCount = 0,
                AggregateId = aggregateId ?? throw new ArgumentNullException(nameof(aggregateId)),
                AggregateType = aggregateType ?? throw new ArgumentNullException(nameof(aggregateType)),
                ModuleName = moduleName ?? throw new ArgumentNullException(nameof(moduleName)),
                UserId = userId,
                CorrelationId = correlationId,
                Priority = priority
            };

            _outboxContext.OutboxEvents.Add(outboxEvent);
            await _outboxContext.SaveChangesAsync();

        }
    }
}
