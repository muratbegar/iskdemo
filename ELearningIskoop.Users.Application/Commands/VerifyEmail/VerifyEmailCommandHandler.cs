using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Users.Application.Commands.ChangePassword;
using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Application.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Commands.VerifyEmail
{
  

    public class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand>
    {

        private readonly IUserManager _userManager;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;


        public VerifyEmailCommandHandler(IUserManager userManager, ILogger<ChangePasswordCommandHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Verify email for user: {UserId}", request.UserId);

            var result = await _userManager.VerifyEmailAsync(request.UserId,request.VerificationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Verify email failed for user: {UserId}", request.UserId);
                throw new BusinessRuleViolationException("VERIFY_EMAIL_FAILED", result.Error ?? "Verify email failed");
            }
            _logger.LogInformation("Verify email successfully for user: {UserId}", request.UserId);
        }
    }
}
