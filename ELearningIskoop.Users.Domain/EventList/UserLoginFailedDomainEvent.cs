using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;

namespace ELearningIskoop.Users.Domain.EventList
{
    public record UserLoginFailedDomainEvent : DomainEvent
    {
        public int UserId { get; }
        public int FailedAttempts { get; }

        public UserLoginFailedDomainEvent(int userId, int failedAttempts)
        {
            UserId = userId;
            FailedAttempts = failedAttempts;
        }

        public override string ToString()
        {
            return $"Login failed for user ID {UserId}. Total failed attempts: {FailedAttempts}";
        }

    }
}
