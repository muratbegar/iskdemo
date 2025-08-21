using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Shared.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Domain.EventList
{
    public record UserEmailVerifiedDomainEvent : DomainEvent
    {
        public int UserId { get; }
        public Email Email { get; }

        public UserEmailVerifiedDomainEvent(int userId, Email email)
        {
            UserId = userId;
            Email = email;
        }

        public override string ToString()
        {
            return $"Email verified for user ID {UserId} ({Email.Value})";
        }
    }
}
