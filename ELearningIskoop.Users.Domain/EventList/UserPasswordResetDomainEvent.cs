using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;

namespace ELearningIskoop.Users.Domain.EventList
{
    public record UserPasswordResetDomainEvent : DomainEvent
    {
        public int UserId { get; }

        public UserPasswordResetDomainEvent(int userId)
        {
            UserId = userId;
        }

        public override string ToString()
        {
            return $"Password reset for user ID {UserId}";
        }
    }
}
