using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Domain.ValueObjects.Permissions
{
    public class Resource : IEquatable<Resource>
    {

        public string Value { get; private set; }

        // Predefined resources
        public static readonly Resource Users = new("users");
        public static readonly Resource Roles = new("roles");
        public static readonly Resource Courses = new("courses");
        public static readonly Resource Payments = new("payments");
        public static readonly Resource System = new("system");
        public static readonly Resource Reports = new("reports");
        public static readonly Resource Analytics = new("analytics");

        private static readonly HashSet<string> AllowedResources = new()
        {
            "users", "roles", "courses", "payments", "system", "reports", "analytics"
        };

        private Resource(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Resource cannot be empty");

            var normalizedValue = value.ToLowerInvariant().Trim();

            if (!AllowedResources.Contains(normalizedValue))
                throw new ArgumentException($"Invalid resource: {value}. Allowed: {string.Join(", ", AllowedResources)}");

            Value = normalizedValue;
        }

        public static Resource From(string value) => new(value);

        public static Resource[] GetAllResources() =>
            AllowedResources.Select(r => new Resource(r)).ToArray();

        public bool Equals(Resource other) =>
            other != null && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);

        public override bool Equals(object obj) => Equals(obj as Resource);
        public override int GetHashCode() => Value.ToLowerInvariant().GetHashCode();
        public override string ToString() => Value;

        public static implicit operator string(Resource resource) => resource.Value;
        public static implicit operator Resource(string value) => From(value);
    }
}
