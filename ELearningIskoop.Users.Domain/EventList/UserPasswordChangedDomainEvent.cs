using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;

namespace ELearningIskoop.Users.Domain.EventList
{
    public record UserPasswordChangedDomainEvent : DomainEvent
    {
        public int UserId { get; }

        public UserPasswordChangedDomainEvent(int userId)
        {
            UserId = userId;
        }

        public override string ToString()
        {
            return $"Password changed for user ID {UserId}";
        }
    }
}
