using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Users.Domain.EnumList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Domain.EventList
{
    public record UserStatusChangedDomainEvent : DomainEvent
    {
        public int UserId { get; }
        public UserStatus OldStatus { get; }
        public UserStatus NewStatus { get; }

        public UserStatusChangedDomainEvent(int userId, UserStatus oldStatus, UserStatus newStatus)
        {
            UserId = userId;
            OldStatus = oldStatus;
            NewStatus = newStatus;
        }

        public override string ToString()
        {
            return $"User ID {UserId} status changed from {OldStatus} to {NewStatus}";
        }
    }
}
