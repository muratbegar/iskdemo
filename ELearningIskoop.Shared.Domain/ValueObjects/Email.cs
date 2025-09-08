using ELearningIskoop.BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ELearningIskoop.Shared.Domain.ValueObjects
{
    // Email adresi value object'i
    // Tüm modüllerde kullanılan email validation ve işlemleri
    [Owned]
    public class Email : IEquatable<Email>
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // EF Core için private set gerekli
        public string Value { get; private set; }

        // EF Core için parameterless constructor
        protected Email()
        {
            Value = string.Empty;
        }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be null or empty.", nameof(value));

            value = value.Trim().ToLowerInvariant();

            if (!IsValid(value))
                throw new ArgumentException($"Invalid email format: {value}", nameof(value));

            Value = value;
        }

        public static bool IsValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return EmailRegex.IsMatch(email.Trim());
        }

        public static Email Parse(string email) => new Email(email);

        public static bool TryParse(string email, out Email? result)
        {
            result = null;

            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                result = new Email(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Equality members
        public bool Equals(Email? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Email)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode(StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator ==(Email? left, Email? right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);
            return left.Equals(right);
        }

        public static bool operator !=(Email? left, Email? right) => !(left == right);

        // Implicit/Explicit conversions
        public static implicit operator string(Email email) => email?.Value ?? string.Empty;

        public static explicit operator Email(string value) => new Email(value);

        public override string ToString() => Value;

        // Domain ve LocalPart özellikleri (opsiyonel)
        public string Domain => Value.Split('@')[1];
        public string LocalPart => Value.Split('@')[0];
    }

}
