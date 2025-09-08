using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Application.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand>
    {
        private readonly IUserManager _userManager;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;

        public ChangePasswordCommandHandler(IUserManager userManager, ILogger<ChangePasswordCommandHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Changing password for user: {UserId}", request.UserId);

            var result = await _userManager.ChangePasswordAsync(
                request.UserId,
                request.CurrentPassword,
                request.NewPassword
            );

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Password change failed for user: {UserId}", request.UserId);
                throw new BusinessRuleViolationException("PASSWORD_CHANGE_FAILED", result.Error ?? "Password change failed");
            }

            _logger.LogInformation("Password changed successfully for user: {UserId}", request.UserId);
        }
    }
}
