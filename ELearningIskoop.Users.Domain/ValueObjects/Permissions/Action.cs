using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Domain.ValueObjects.Permissions
{
    public class Action : IEquatable<Action>
    {
        public string Value { get; private set; }
        public int HierarchyLevel { get; private set; }

        // Predefined actions with hierarchy (higher number = more permissions)
        public static readonly Action Read = new("read", 1);
        public static readonly Action Write = new("write", 2);
        public static readonly Action Delete = new("delete", 3);
        public static readonly Action Admin = new("admin", 4);

        private static readonly Dictionary<string, int> ActionHierarchy = new()
        {
            { "read", 1 },
            { "write", 2 },
            { "delete", 3 },
            { "admin", 4 }
        };

        private Action(string value, int hierarchyLevel)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Action cannot be empty");

            var normalizedValue = value.ToLowerInvariant().Trim();

            if (!ActionHierarchy.ContainsKey(normalizedValue))
                throw new ArgumentException($"Invalid action: {value}. Allowed: {string.Join(", ", ActionHierarchy.Keys)}");

            Value = normalizedValue;
            HierarchyLevel = hierarchyLevel;
        }

        public static Action From(string value)
        {
            var normalizedValue = value.ToLowerInvariant().Trim();
            return new Action(normalizedValue, ActionHierarchy[normalizedValue]);
        }

        public static Action[] GetAllActions() =>
            ActionHierarchy.Select(kvp => new Action(kvp.Key, kvp.Value)).ToArray();

        // Hierarchy check: admin includes write, delete, read
        public bool Includes(Action other)
        {
            return HierarchyLevel >= other.HierarchyLevel;
        }

        // Get all actions that this action includes
        public Action[] GetIncludedActions()
        {
            return ActionHierarchy
                .Where(kvp => kvp.Value <= HierarchyLevel)
                .Select(kvp => new Action(kvp.Key, kvp.Value))
                .ToArray();
        }

        public bool Equals(Action other) =>
            other != null && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);

        public override bool Equals(object obj) => Equals(obj as Action);
        public override int GetHashCode() => Value.ToLowerInvariant().GetHashCode();
        public override string ToString() => Value;

        public static implicit operator string(Action action) => action.Value;
        public static implicit operator Action(string value) => From(value);
    }
}
