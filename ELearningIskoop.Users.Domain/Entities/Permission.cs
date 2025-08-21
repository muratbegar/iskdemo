using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;

namespace ELearningIskoop.Users.Domain.Entities
{
    public class Permission : BaseEntity
    {
        protected Permission() { }

        private Permission(int roleId, string permission)
        {
            RoleId = roleId;
            Permissions = permission;
            IsActive = true;
        }

        public int RoleId { get; private set; }
        public string Permissions { get; private set; }
        public bool IsActive { get; private set; }

        public Role Role { get; private set; }

        public static Permission Create(int roleId, string permission)
        {
            return new Permission(roleId, permission);
        }

    }
}
