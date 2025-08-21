using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Shared.Domain.ValueObjects;

namespace ELearningIskoop.Users.Domain.EventList
{
    public record UserCreatedDomainEvent : DomainEvent
    {
        public int UserId { get; }
        public Email Email { get; }

        public UserCreatedDomainEvent(int userId,Email email)
        {
            UserId = userId;
            Email = email ?? throw new ArgumentNullException(nameof(email), "Email cannot be null");
        }

        public override string ToString()
        {
            return $"User created: {Email.Value} (ID: {UserId})";
        }
    }
}
