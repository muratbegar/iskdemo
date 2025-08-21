using ELearningIskoop.BuildingBlocks.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Domain.EventList
{
    public record UserLoggedDomainEvent : DomainEvent
    {
        public int UserId { get; }
        public string UserName { get; }
        public string IpAddress { get; }
        public DateTime LoginTime { get; }


        public UserLoggedDomainEvent(int userId, string userName, DateTime loginTime)
        {
            UserId = userId;
            UserName = userName ?? throw new ArgumentNullException(nameof(userName), "UserName cannot be null");
            LoginTime = loginTime;
        }
        public UserLoggedDomainEvent(int userId, string ipAddress)
        {
            UserId = userId;
            IpAddress = ipAddress;
        }
        protected UserLoggedDomainEvent(int userId, string ipAddress, DateTime loginTime, DateTime occurredOn, Guid eventId)
            : base(occurredOn, eventId)
        {
            UserId = userId;
            IpAddress = ipAddress;
            LoginTime = loginTime;
        }
        public override string ToString()
        {
            return $"User logged in: ID {UserId} from {IpAddress} at {LoginTime:yyyy-MM-dd HH:mm:ss}";
        }
    }
}
