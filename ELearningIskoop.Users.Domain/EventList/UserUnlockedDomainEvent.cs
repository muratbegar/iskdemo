using ELearningIskoop.BuildingBlocks.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Domain.EventList
{
    public record UserUnlockedDomainEvent : DomainEvent
    {
        public int UserId { get; }

        public UserUnlockedDomainEvent(int userId)
        {
            UserId = userId;
        }

        public override string ToString()
        {
            return $"User ID {UserId} unlocked";
        }
    }
}
