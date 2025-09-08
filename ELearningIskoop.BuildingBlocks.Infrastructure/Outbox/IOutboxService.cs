using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Infrastructure.Outbox
{
    public interface IOutboxService
    {
        Task PublishAsync<T>(T domainEvent, string aggregateId, string aggregateType, string moduleName,
            string userId = null, string correlationId = null, int priority = 0) where T : class;
    }
}
