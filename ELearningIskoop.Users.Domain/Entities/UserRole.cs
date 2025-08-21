using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Users.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Domain.Entities
{
    public class UserRole : BaseEntity
    {
        protected UserRole() { }

        private UserRole(int userId, int roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }

        public int UserId { get; set; }
        public int RoleId { get; set; }

        public User User { get; private set; } = null;
        public Role Role { get; private set; } = null;

        public static UserRole Create(int userId, int roleId)
        {
            return new UserRole(userId, roleId);
        }
    }

}
