using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Infrastructure.Outbox
{
    public class OutboxStatistics
    {
        public int PendingEvents { get; set; }
        public int ProcessedToday { get; set; }
        public int FailedEvents { get; set; }
        public DateTime? OldestPendingEvent { get; set; }
        public Dictionary<string, int> EventsByModule { get; set; } = new();
        public Dictionary<string, int> EventsByType { get; set; } = new();
        public Dictionary<int, int> EventsByPriority { get; set; } = new();
    }
}
