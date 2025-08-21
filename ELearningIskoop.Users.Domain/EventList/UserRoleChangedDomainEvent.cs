using ELearningIskoop.BuildingBlocks.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Domain.EventList
{
    public record UserRoleRemovedDomainEvent : DomainEvent
    {
        public int UserId { get; }
        public int RoleId { get; }

        public UserRoleRemovedDomainEvent(int userId, int roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }

        public override string ToString()
        {
            return $"Role ID {RoleId} removed from user ID {UserId}";
        }
    }
}
