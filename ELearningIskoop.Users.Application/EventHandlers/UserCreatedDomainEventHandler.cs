using ELearningIskoop.Users.Application.Services;
using ELearningIskoop.Users.Domain.EventList;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.EventHandlers
{
    public class UserCreatedDomainEventHandler : INotificationHandler<UserCreatedDomainEvent>, INotification
    {
        private readonly IEmailServices _emailServices;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserCreatedDomainEventHandler> _logger;

        public UserCreatedDomainEventHandler(
            IEmailServices emailServices,
            ILogger<UserCreatedDomainEventHandler> logger,
            IConfiguration configuration)
        {
            _emailServices = emailServices;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling UserCreatedDomainEvent for user {UserId}", notification.UserId);

            try
            {
                //mail için doğrulama kodu oluşuturma

                // Send verification email


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send verification email for user {UserId}", notification.UserId);
            }
        }
    }
}
