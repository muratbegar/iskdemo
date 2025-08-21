using ELearningIskoop.BuildingBlocks.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Domain.EventList
{
    public record UserRoleAssignedDomainEvent : DomainEvent
    {
        public int UserId { get; }
        public string RoleName { get; }

        public UserRoleAssignedDomainEvent(int userId, string roleName)
        {
            UserId = userId;
            RoleName = roleName;
        }

        public override string ToString()
        {
            return $"Role '{RoleName}' assigned to user ID {UserId}";
        }
    }
}
