using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Shared.Infrastructure.Outbox
{
    public class OutboxEvent
    {
        public int Id { get; set; }
        public string EventType { get; set; }
        public string EventData { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ProcessedAt { get; set; }
        public bool Processed { get; set; }
        public int RetryCount { get; set; }
        public int MaxRetries { get; set; }

        public string? LastError { get; set; }
        public DateTime? NextRetryAt { get; set; }

        public string AggregateId { get; set; }
        public string AggregateType { get; set; }

        public string ModuleName { get; set; } // "Users", "Courses", "Payments", etc.
        public string CorrelationId { get; set; } // Tracing için
        public string UserId { get; set; } // Hangi user trigger etmiş
        public int Priority { get; set; } = 0; // 0=Normal, 1=High, 2=Critical
    }
}
