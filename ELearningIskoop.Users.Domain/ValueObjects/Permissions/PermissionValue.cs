using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Domain.ValueObjects.Permissions
{
    public class PermissionValue : IEquatable<PermissionValue>
    {
        public Resource Resource { get; private set; }
        public Action Action { get; private set; }
        public Scope Scope { get; private set; }

        private PermissionValue(Resource resource, Action action, Scope scope)
        {
            Resource = resource ?? throw new ArgumentNullException(nameof(resource));
            Action = action ?? throw new ArgumentNullException(nameof(action));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public static PermissionValue Create(Resource resource, Action action, Scope scope)
        {
            return new PermissionValue(resource, action, scope);
        }

        public static PermissionValue Parse(string permissionString)
        {
            if (string.IsNullOrWhiteSpace(permissionString))
                throw new ArgumentException("Permission string cannot be empty");

            var parts = permissionString.Split(':');
            if (parts.Length != 3)
                throw new ArgumentException("Permission string must be in format 'resource:action:scope'");

            var resource = Resource.From(parts[0].Trim());
            var action = Action.From(parts[1].Trim());
            var scope = Scope.From(parts[2].Trim());

            return new PermissionValue(resource, action, scope);
        }

        // Permission hierarchy check: does this permission include the other?
        public bool Includes(PermissionValue other)
        {
            if (other == null) return false;

            // Same resource required
            if (!Resource.Equals(other.Resource)) return false;

            // Action and scope must be equal or higher in hierarchy
            return Action.Includes(other.Action) && Scope.Includes(other.Scope);
        }

        // Generate all permissions that this permission includes
        public PermissionValue[] GetIncludedPermissions()
        {
            var includedActions = Action.GetIncludedActions();
            var includedScopes = Scope.GetIncludedScopes();
            var results = new List<PermissionValue>();

            foreach (var action in includedActions)
            {
                foreach (var scope in includedScopes)
                {
                    results.Add(new PermissionValue(Resource, action, scope));
                }
            }

            return results.ToArray();
        }

        public bool Equals(PermissionValue other)
        {
            if (other == null) return false;
            return Resource.Equals(other.Resource) &&
                   Action.Equals(other.Action) &&
                   Scope.Equals(other.Scope);
        }

        public override bool Equals(object obj) => Equals(obj as PermissionValue);

        public override int GetHashCode() =>
            HashCode.Combine(Resource.GetHashCode(), Action.GetHashCode(), Scope.GetHashCode());

        public override string ToString() => $"{Resource}:{Action}:{Scope}";

        // Predefined common permissions
        public static class Common
        {
            // Users
            public static readonly PermissionValue UsersReadOwn = Create(Resource.Users, Action.Read, Scope.Own);
            public static readonly PermissionValue UsersWriteOwn = Create(Resource.Users, Action.Write, Scope.Own);
            public static readonly PermissionValue UsersReadAll = Create(Resource.Users, Action.Read, Scope.All);
            public static readonly PermissionValue UsersAdminAll = Create(Resource.Users, Action.Admin, Scope.All);

            // Roles
            public static readonly PermissionValue RolesReadAll = Create(Resource.Roles, Action.Read, Scope.All);
            public static readonly PermissionValue RolesAdminAll = Create(Resource.Roles, Action.Admin, Scope.All);

            // System
            public static readonly PermissionValue SystemAdminAll = Create(Resource.System, Action.Admin, Scope.All);
        }
    }
}
