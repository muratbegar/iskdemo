using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace ELearningIskoop.BuildingBlocks.Domain
{
    public abstract record DomainEvent : IDomainEvent,INotification
    {
        protected DomainEvent()
        {
            EventId = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            EventType = GetType().Name;
        }
        protected DomainEvent(DateTime occurredOn, Guid eventId)
        {
            OccurredOn = occurredOn;
            EventId = eventId;
        }
        public Guid EventId { get; }
        public DateTime OccurredOn { get; }
        public string EventType { get; }
    }
}
