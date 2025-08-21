using ELearningIskoop.BuildingBlocks.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Domain.EventList
{
    public record UserLockedDomainEvent : DomainEvent
    {
        public int UserId { get; }
        public DateTime LockedUntil { get; }

        public UserLockedDomainEvent(int userId, DateTime lockedUntil)
        {
            UserId = userId;
            LockedUntil = lockedUntil;
        }

        public override string ToString()
        {
            return $"User ID {UserId} locked until {LockedUntil:yyyy-MM-dd HH:mm:ss}";
        }
    }
}
