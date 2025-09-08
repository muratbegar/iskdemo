using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Users.Domain.ValueObjects.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = ELearningIskoop.Users.Domain.ValueObjects.Permissions.Action;

namespace ELearningIskoop.Users.Domain.Entities
{
    public class Permission : BaseEntity
    {
        protected Permission() { }

        private Permission(int roleId, PermissionValue permissionValue)
        {
            RoleId = roleId;
            PermissionValue = permissionValue ?? throw new ArgumentNullException(nameof(permissionValue));
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public int RoleId { get; private set; }
        public PermissionValue PermissionValue { get; private set; }
        public bool IsActive { get; private set; }

        public Role Role { get; private set; }

        public static Permission Create(int roleId, PermissionValue permissionValue)
        {
            return new Permission(roleId, permissionValue);
        }

        // Convenience factory methods
        public static Permission Create(int roleId, Resource resource, Action action, Scope scope)
        {
            var permissionValue = PermissionValue.Create(resource, action, scope);
            return new Permission(roleId, permissionValue);
        }
        public static Permission CreateFromString(int roleId, string permissionString)
        {
            var permissionValue = PermissionValue.Parse(permissionString);
            return new Permission(roleId, permissionValue);
        }

        // Business methods
        public void Activate()
        {
            if (IsActive)
                throw new InvalidOperationException("Permission is already active");

            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new InvalidOperationException("Permission is already inactive");

            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        // Permission hierarchy checks
        public bool Includes(Permission other)
        {
            if (other == null) return false;
            if (RoleId != other.RoleId) return false;
            if (!IsActive || !other.IsActive) return false;

            return PermissionValue.Includes(other.PermissionValue);
        }

        public bool Includes(PermissionValue permissionValue)
        {
            if (!IsActive) return false;
            return PermissionValue.Includes(permissionValue);
        }

        // Get all permissions this permission includes (via hierarchy)
        public PermissionValue[] GetIncludedPermissionValues()
        {
            return IsActive ? PermissionValue.GetIncludedPermissions() : new PermissionValue[0];
        }

        // Equality based on RoleId and PermissionValue
        public override bool Equals(object obj)
        {
            if (obj is not Permission other) return false;
            return RoleId == other.RoleId && PermissionValue.Equals(other.PermissionValue);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RoleId, PermissionValue);
        }

        public override string ToString()
        {
            return $"Role:{RoleId} -> {PermissionValue} (Active:{IsActive})";
        }

    }
}
