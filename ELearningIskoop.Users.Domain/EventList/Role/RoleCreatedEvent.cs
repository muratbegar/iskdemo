using ELearningIskoop.BuildingBlocks.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace ELearningIskoop.Users.Domain.EventList.Role
{
    public class RoleCreatedEvent : IDomainEvent, INotification
    {
        public int RoleId { get; }
        public string RoleName { get; }
        public string CreatedBy { get; }
        public DateTime OccurredAt { get; }

        public RoleCreatedEvent(int roleId, string roleName, string createdBy)
        {
            RoleId = roleId;
            RoleName = roleName;
            CreatedBy = createdBy;
            OccurredAt = DateTime.UtcNow;
        }

        public Guid EventId { get; }
        public DateTime OccurredOn { get; }
        public string EventType { get; }
    }
}
