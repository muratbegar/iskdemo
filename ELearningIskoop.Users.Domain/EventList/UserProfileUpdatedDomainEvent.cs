using ELearningIskoop.BuildingBlocks.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Domain.EventList
{
    public record UserProfileUpdatedDomainEvent : DomainEvent
    {
        public int UserId { get; }

        public UserProfileUpdatedDomainEvent(int userId)
        {
            UserId = userId;
        }

        public override string ToString()
        {
            return $"Profile updated for user ID {UserId}";
        }
    }
}
