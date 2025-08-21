using ELearningIskoop.BuildingBlocks.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ELearningIskoop.Users.Domain.Entities
{
    public class Role : BaseEntity
    {
        private readonly List<UserRole> _userRoles = new();
        private readonly List<Permission> _permissions = new();

        protected Role() { }

        private Role(string name, string description)
        {
            Name = name;
            NormalizedName = name.ToUpperInvariant();
            Description = description;
            IsActive = true;
        }

        public string Name { get; private set; } = string.Empty;
        public string NormalizedName { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }

        public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();
        public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

        public static Role Create(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name cannot be empty");

            return new Role(name, description);

        }

        public void AddPermission(string permission)
        {
            if (_permissions.Any(p => p.Permissions == permission))
                return;

            _permissions.Add(Permission.Create(ObjectId, permission));
        }

        public bool HasPermission(string permission)
        {
            return _permissions.Any(p => p.Permissions == permission && p.IsActive);
        }

    }

}
