using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Users.Domain.ValueObjects.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Action = ELearningIskoop.Users.Domain.ValueObjects.Permissions.Action;

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
            CreatedAt = DateTime.UtcNow;
            IsDeleted = false;
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

        public void AddPermission(PermissionValue permissionValue)
        {
            if (IsDeleted)
                throw new InvalidOperationException("Cannot add permission to deleted role");

            if (permissionValue == null)
                throw new ArgumentNullException(nameof(permissionValue));

            // Check if permission already exists (active or inactive)
            var existingPermission = _permissions.FirstOrDefault(p =>
                p.PermissionValue.Equals(permissionValue));

            if (existingPermission != null)
            {
                if (existingPermission.IsActive)
                    throw new InvalidOperationException($"Permission '{permissionValue}' already exists for role '{Name}'");

                // Reactivate if it was deactivated
                existingPermission.Activate();
            }
            else
            {
                // Add new permission
                var permission = Permission.Create(ObjectId, permissionValue);
                _permissions.Add(permission);
            }

            UpdatedAt = DateTime.UtcNow;
        }
        public void AddPermission(Resource resource, Action action, Scope scope)
        {
            var permissionValue = PermissionValue.Create(resource, action, scope);
            AddPermission(permissionValue);
        }

        public void AddPermission(string permissionString)
        {
            var permissionValue = PermissionValue.Parse(permissionString);
            AddPermission(permissionValue);
        }
        public void UpdateName(string newName)
        {
            if(IsDeleted)
                throw new InvalidOperationException("Cannot update a deleted role.");
            if(string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Role name cannot be empty.");
            
            Name = newName.Trim();
            UpdatedAt = DateTime.UtcNow;
        }
        public void RemovePermission(PermissionValue permissionValue)
        {
            if (IsDeleted)
                throw new InvalidOperationException("Cannot remove permission from deleted role");

            if (permissionValue == null)
                throw new ArgumentNullException(nameof(permissionValue));

            var permission = _permissions.FirstOrDefault(p =>
                p.PermissionValue.Equals(permissionValue) && p.IsActive);

            if (permission == null)
                throw new InvalidOperationException($"Permission '{permissionValue}' not found for role '{Name}'");

            permission.Deactivate();
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemovePermission(Resource resource, Action action, Scope scope)
        {
            var permissionValue = PermissionValue.Create(resource, action, scope);
            RemovePermission(permissionValue);
        }

        public void RemovePermission(string permissionString)
        {
            var permissionValue = PermissionValue.Parse(permissionString);
            RemovePermission(permissionValue);
        }

        public void UpdateDescription(string newDescription)
        {
            if (IsDeleted)
                throw new InvalidOperationException("Cannot update deleted role");

            Description = newDescription;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsDeleted()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Role is already deleted");

            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool HasPermission(PermissionValue permissionValue)
        {
            if (IsDeleted || permissionValue == null) return false;

            // Direct permission check
            var directPermission = _permissions.FirstOrDefault(p =>
                p.IsActive && p.PermissionValue.Equals(permissionValue));

            if (directPermission != null) return true;

            // Hierarchy check - any permission that includes the requested permission
            return _permissions.Any(p => p.IsActive && p.Includes(permissionValue));
        }

        public bool HasPermission(Resource resource, Action action, Scope scope)
        {
            var permissionValue = PermissionValue.Create(resource, action, scope);
            return HasPermission(permissionValue);
        }

        public bool HasPermission(string permissionString)
        {
            var permissionValue = PermissionValue.Parse(permissionString);
            return HasPermission(permissionValue);
        }

        public List<PermissionValue> GetActivePermissions()
        {
            return _permissions
                .Where(p => p.IsActive)
                .Select(p => p.PermissionValue)
                .ToList();
        }

        public List<PermissionValue> GetEffectivePermissions()
        {
            if (IsDeleted) return new List<PermissionValue>();

            var effectivePermissions = new HashSet<PermissionValue>();

            // Get all active permissions and their included permissions via hierarchy
            foreach (var permission in _permissions.Where(p => p.IsActive))
            {
                effectivePermissions.Add(permission.PermissionValue);

                // Add all permissions included by this permission via hierarchy
                var includedPermissions = permission.GetIncludedPermissionValues();
                foreach (var includedPermission in includedPermissions)
                {
                    effectivePermissions.Add(includedPermission);
                }
            }

            return effectivePermissions.ToList();
        }

        public List<PermissionValue> GetPermissionsByResource(Resource resource)
        {
            return _permissions
                .Where(p => p.IsActive && p.PermissionValue.Resource.Equals(resource))
                .Select(p => p.PermissionValue)
                .ToList();
        }

        public List<PermissionValue> GetPermissionsByAction(Action action)
        {
            return _permissions
                .Where(p => p.IsActive && p.PermissionValue.Action.Equals(action))
                .Select(p => p.PermissionValue)
                .ToList();
        }

        public List<PermissionValue> GetPermissionsByScope(Scope scope)
        {
            return _permissions
                .Where(p => p.IsActive && p.PermissionValue.Scope.Equals(scope))
                .Select(p => p.PermissionValue)
                .ToList();
        }
        public void AddPermissions(IEnumerable<PermissionValue> permissionValues)
        {
            if (permissionValues == null)
                throw new ArgumentNullException(nameof(permissionValues));

            foreach (var permissionValue in permissionValues)
            {
                AddPermission(permissionValue);
            }
        }

        public void ReplacePermissions(IEnumerable<PermissionValue> newPermissions)
        {
            if (IsDeleted)
                throw new InvalidOperationException("Cannot replace permissions of deleted role");

            if (newPermissions == null)
                throw new ArgumentNullException(nameof(newPermissions));

            // Deactivate all current permissions
            foreach (var permission in _permissions.Where(p => p.IsActive))
            {
                permission.Deactivate();
            }

            // Add new permissions
            AddPermissions(newPermissions);
        }

        public void ClearPermissions()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Cannot clear permissions of deleted role");

            foreach (var permission in _permissions.Where(p => p.IsActive))
            {
                permission.Deactivate();
            }

            UpdatedAt = DateTime.UtcNow;
        }
        public static Role CreateAdminRole()
        {
            var adminRole = Create("Admin", "Full system administrator");

            // Add comprehensive admin permissions
            adminRole.AddPermission(PermissionValue.Common.SystemAdminAll);
            adminRole.AddPermission(PermissionValue.Common.UsersAdminAll);
            adminRole.AddPermission(PermissionValue.Common.RolesAdminAll);

            return adminRole;
        }

        public static Role CreateUserRole()
        {
            var userRole = Create("User", "Basic user permissions");

            // Add basic user permissions
            userRole.AddPermission(PermissionValue.Common.UsersReadOwn);
            userRole.AddPermission(PermissionValue.Common.UsersWriteOwn);

            return userRole;
        }
    }

}
