using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Users.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Domain.Entities
{
    public class UserLogin : BaseEntity
    {
        protected UserLogin() { }

        private UserLogin(int userId, string ipAddress, string? userAgent, bool isSuccessful)
        {
            UserId = userId;
            IpAddress = ipAddress;
            UserAgent = userAgent;
            IsSuccessful = isSuccessful;
            LoginAt = DateTime.UtcNow;
        }


        public int UserId { get; private set; }
        public string IpAddress { get; private set; }
        public string? UserAgent { get; private set; }
        public bool IsSuccessful { get; private set; }
        public DateTime LoginAt { get; private set; }
        public User User { get; private set; }

        public static UserLogin Create(int userId, string ipAddress, string? userAgent, bool isSuccessful)
        {
            return new UserLogin(userId, ipAddress, userAgent, isSuccessful);
        }
    }

}
