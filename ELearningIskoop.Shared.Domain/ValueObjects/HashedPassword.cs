using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ELearningIskoop.Shared.Domain.ValueObjects
{
    public class HashedPassword : ValueObject
    {
        public string Hash { get; private set; } = string.Empty;
        public string Salt { get; private set; } = string.Empty;

        // EF Core için parametresiz constructor
        private HashedPassword() { }

        private HashedPassword(string hash, string salt)
        {
            Hash = hash;
            Salt = salt;
        }

        public static HashedPassword Create(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentException("Şifre boş olamaz");

            var salt = GenerateSalt();
            var hash = HashPassword(plainPassword, salt);

            return new HashedPassword(hash, salt);
        }

        public bool Verify(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                return false;

            var hash = HashPassword(plainPassword, Salt);
            return Hash.Equals(hash, StringComparison.Ordinal);
        }

        private static string GenerateSalt()
        {
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            var saltBytes = new byte[32];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        private static string HashPassword(string password, string salt)
        {
            using var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(
                password,
                Convert.FromBase64String(salt),
                10000,
                System.Security.Cryptography.HashAlgorithmName.SHA256);

            var hash = pbkdf2.GetBytes(32);
            return Convert.ToBase64String(hash);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Hash;
            yield return Salt;
        }
    }


}
