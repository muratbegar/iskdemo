using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;

namespace ELearningIskoop.Users.Domain.Entities
{
    public class PasswordResetToken : BaseEntity
    {
        public int UserId{ get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }

        private PasswordResetToken() { } // EF için

        public PasswordResetToken(int userId, string token, DateTime expiresAt)
        {
            UserId = userId;
            Token = token;
            ExpiresAt = expiresAt;
            IsUsed = false;
        }

        public void MarkAsUsed()
        {
            IsUsed = true;
        }
    }
}
