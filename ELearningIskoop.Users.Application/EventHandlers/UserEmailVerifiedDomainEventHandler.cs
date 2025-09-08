using ELearningIskoop.Users.Domain.EventList;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Application.Services;
using ELearningIskoop.Users.Domain.Repos;
using Microsoft.Extensions.Logging;

namespace ELearningIskoop.Users.Application.EventHandlers
{
    public class UserEmailVerifiedDomainEventHandler : INotificationHandler<UserEmailVerifiedDomainEvent>
    {
       
        private readonly IEmailServices _emailService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserEmailVerifiedDomainEventHandler> _logger;

        public UserEmailVerifiedDomainEventHandler(
            IEmailServices emailService,
            IUserRepository userRepository,
            ILogger<UserEmailVerifiedDomainEventHandler> logger)
        {
            _emailService = emailService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task Handle(UserEmailVerifiedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Email verified for user {UserId}", notification.UserId);

            try
            {
                //var user = await _userRepository.GetByIdAsync(notification.UserId, cancellationToken);
                //if (user != null)
                //{
                //    await _emailService.SendWelcomeEmailAsync(
                //        notification.Email.Value,
                //        user.Name.FirstName
                //    );
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email for user {UserId}", notification.UserId);
            }
        }
    }
}

