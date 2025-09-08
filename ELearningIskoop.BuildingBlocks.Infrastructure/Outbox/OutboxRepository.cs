using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;


namespace ELearningIskoop.Shared.Infrastructure.Outbox
{
    public class OutboxRepository : IOutboxRepository
    {

        private readonly OutboxDbContext _context;
        public OutboxRepository(OutboxDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<List<OutboxEvent>> GetUnprocessedEventsAsync(int batchSize = 50)
        {
            return await _context.OutboxEvents
                .Where(e => !e.Processed &&
                            (e.NextRetryAt == null || e.NextRetryAt <= DateTime.UtcNow))
                .OrderBy(e => e.Priority) // Higher priority first (2, 1, 0)
                .ThenBy(e => e.CreatedAt)
                .Take(batchSize)
                .ToListAsync();
        }

        public async Task<List<OutboxEvent>> GetUnprocessedEventsByModuleAsync(string moduleName, int batchSize = 50)
        {
            return await _context.OutboxEvents
                .Where(e => !e.Processed &&
                            e.ModuleName == moduleName &&
                            (e.NextRetryAt == null || e.NextRetryAt <= DateTime.UtcNow))
                .OrderBy(e => e.Priority)
                .ThenBy(e => e.CreatedAt)
                .Take(batchSize)
                .ToListAsync();
        }

        public async Task MarkAsProcessedAsync(long eventId)
        {
            var outboxEvent = await _context.OutboxEvents.FindAsync(eventId);
            if (outboxEvent != null)
            {
                outboxEvent.Processed = true;
                outboxEvent.ProcessedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAsFailedAsync(long eventId, string error)
        {
            var outboxEvent = await _context.OutboxEvents.FindAsync(eventId);
            if (outboxEvent != null)
            {
                outboxEvent.RetryCount++;
                outboxEvent.LastError = error?.Length > 2000 ? error.Substring(0, 2000) : error;

                if (outboxEvent.RetryCount < outboxEvent.MaxRetries)
                {
                    // Options'tan alınacak base delay ile exponential backoff
                    // Bu değer şimdilik hard-coded, ileride options'tan alınabilir
                    var delayMinutes = Math.Pow(2, outboxEvent.RetryCount) * 2; // 2min, 8min, 32min
                    outboxEvent.NextRetryAt = DateTime.UtcNow.AddMinutes(delayMinutes);
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<OutboxEvent>> GetFailedEventsAsync()
        {
            return await _context.OutboxEvents
                .Where(e => !e.Processed && e.RetryCount >= e.MaxRetries)
                .OrderBy(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<OutboxEvent>> GetFailedEventsByModuleAsync(string moduleName)
        {
            return await _context.OutboxEvents
                .Where(e => !e.Processed && e.RetryCount >= e.MaxRetries && e.ModuleName == moduleName)
                .OrderBy(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task DeleteProcessedEventsOlderThanAsync(DateTime cutoffDate)
        {
            var oldEvents = await _context.OutboxEvents
                .Where(e => e.Processed && e.ProcessedAt < cutoffDate)
                .ToListAsync();

            _context.OutboxEvents.RemoveRange(oldEvents);
            await _context.SaveChangesAsync();
        }

        public async Task<OutboxStatistics> GetStatisticsAsync()
        {
            var stats = new OutboxStatistics();

            var allEvents = await _context.OutboxEvents.ToListAsync();

            stats.PendingEvents = allEvents.Count(e => !e.Processed);
            stats.ProcessedToday = allEvents.Count(e => e.Processed && e.ProcessedAt.Date == DateTime.Today);
            stats.FailedEvents = allEvents.Count(e => !e.Processed && e.RetryCount >= e.MaxRetries);
            stats.OldestPendingEvent = allEvents.Where(e => !e.Processed).Min(e => e.CreatedAt as DateTime?);

            stats.EventsByModule = allEvents
                .GroupBy(e => e.ModuleName)
                .ToDictionary(g => g.Key, g => g.Count());

            stats.EventsByType = allEvents
                .GroupBy(e => e.AggregateType)
                .ToDictionary(g => g.Key, g => g.Count());

            stats.EventsByPriority = allEvents
                .GroupBy(e => e.Priority)
                .ToDictionary(g => g.Key, g => g.Count());

            return stats;
        }

        public async Task<OutboxStatistics> GetStatisticsByModuleAsync(string moduleName)
        {
            var stats = new OutboxStatistics();

            var moduleEvents = await _context.OutboxEvents
                .Where(e => e.ModuleName == moduleName)
                .ToListAsync();

            stats.PendingEvents = moduleEvents.Count(e => !e.Processed);
            stats.ProcessedToday = moduleEvents.Count(e => e.Processed && e.ProcessedAt.Date == DateTime.Today);
            stats.FailedEvents = moduleEvents.Count(e => !e.Processed && e.RetryCount >= e.MaxRetries);
            stats.OldestPendingEvent = moduleEvents.Where(e => !e.Processed).Min(e => e.CreatedAt as DateTime?);

            stats.EventsByType = moduleEvents
                .GroupBy(e => e.AggregateType)
                .ToDictionary(g => g.Key, g => g.Count());

            return stats;
        }

        public async Task ResetEventForRetryAsync(long eventId)
        {
            var outboxEvent = await _context.OutboxEvents.FindAsync(eventId);
            if (outboxEvent != null)
            {
                outboxEvent.RetryCount = 0;
                outboxEvent.NextRetryAt = DateTime.UtcNow;
                outboxEvent.LastError = null;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<OutboxEvent> GetByIdAsync(long eventId)
        {
            return await _context.OutboxEvents.FindAsync(eventId);
        }
    }
}
