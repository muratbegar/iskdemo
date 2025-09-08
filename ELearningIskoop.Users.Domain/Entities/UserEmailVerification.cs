using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;

namespace ELearningIskoop.Users.Domain.Entities
{
    public class UserEmailVerification : BaseEntity
    {
        public string UserMail { get; private set; }
        public string Code { get; private set; } = string.Empty;

        public DateTime ExpiresAt { get; private set; }
        public bool IsUsed{ get; private set; }

        private UserEmailVerification() {}

        public UserEmailVerification(string userMail, string code, DateTime expiresAt)
        {
            UserMail = userMail;
            Code = code;
            ExpiresAt = expiresAt;
        }

        public void MarkAsUsed()
        {
            if (IsUsed)
                throw new BusinessRuleViolationException("VERIFICATION_ALREADY_USED", "Verification code already used.");
            IsUsed = true;
        }
        public bool IsValid(string code)
        {
            return !IsUsed && Code == code && ExpiresAt > DateTime.UtcNow;
        }
    }
}
