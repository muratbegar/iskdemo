using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Infrastructure.Outbox;

namespace ELearningIskoop.Shared.Infrastructure.Outbox
{
    public interface IOutboxRepository
    {
        Task<List<OutboxEvent>> GetUnprocessedEventsAsync(int batchSize = 50);
        Task<List<OutboxEvent>> GetUnprocessedEventsByModuleAsync(string moduleName, int batchSize = 50);
        Task MarkAsProcessedAsync(long eventId);
        Task MarkAsFailedAsync(long eventId, string error);
        Task<List<OutboxEvent>> GetFailedEventsAsync();
        Task<List<OutboxEvent>> GetFailedEventsByModuleAsync(string moduleName);
        Task DeleteProcessedEventsOlderThanAsync(DateTime cutoffDate);
        Task<OutboxStatistics> GetStatisticsAsync();
        Task<OutboxStatistics> GetStatisticsByModuleAsync(string moduleName);
        Task ResetEventForRetryAsync(long eventId);
        Task<OutboxEvent> GetByIdAsync(long eventId);
    }
}
