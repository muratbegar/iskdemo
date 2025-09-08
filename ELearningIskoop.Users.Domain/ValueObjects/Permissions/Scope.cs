using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Domain.ValueObjects.Permissions
{
    public class Scope : IEquatable<Scope>
    {
        public string Value { get; private set; }
        public int HierarchyLevel { get; private set; }

        // Predefined scopes with hierarchy (higher number = broader scope)
        public static readonly Scope Own = new("own", 1);
        public static readonly Scope Department = new("department", 2);
        public static readonly Scope Organization = new("organization", 3);
        public static readonly Scope All = new("all", 4);

        private static readonly Dictionary<string, int> ScopeHierarchy = new()
        {
            { "own", 1 },
            { "department", 2 },
            { "organization", 3 },
            { "all", 4 }
        };

        private Scope(string value, int hierarchyLevel)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Scope cannot be empty");

            var normalizedValue = value.ToLowerInvariant().Trim();

            if (!ScopeHierarchy.ContainsKey(normalizedValue))
                throw new ArgumentException($"Invalid scope: {value}. Allowed: {string.Join(", ", ScopeHierarchy.Keys)}");

            Value = normalizedValue;
            HierarchyLevel = hierarchyLevel;
        }

        public static Scope From(string value)
        {
            var normalizedValue = value.ToLowerInvariant().Trim();
            return new Scope(normalizedValue, ScopeHierarchy[normalizedValue]);
        }

        public static Scope[] GetAllScopes() =>
            ScopeHierarchy.Select(kvp => new Scope(kvp.Key, kvp.Value)).ToArray();

        // Hierarchy check: all includes organization, department, own
        public bool Includes(Scope other)
        {
            return HierarchyLevel >= other.HierarchyLevel;
        }

        // Get all scopes that this scope includes
        public Scope[] GetIncludedScopes()
        {
            return ScopeHierarchy
                .Where(kvp => kvp.Value <= HierarchyLevel)
                .Select(kvp => new Scope(kvp.Key, kvp.Value))
                .ToArray();
        }

        public bool Equals(Scope other) =>
            other != null && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);

        public override bool Equals(object obj) => Equals(obj as Scope);
        public override int GetHashCode() => Value.ToLowerInvariant().GetHashCode();
        public override string ToString() => Value;

        public static implicit operator string(Scope scope) => scope.Value;
        public static implicit operator Scope(string value) => From(value);
    }
}
