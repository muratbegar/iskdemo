using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.BuildingBlocks.Application.Results;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Shared.Domain.ValueObjects;
using ELearningIskoop.Users.Application.Commands.ChangePassword;
using ELearningIskoop.Users.Domain.Entities;
using ELearningIskoop.Users.Domain.Repos;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Application.Services;

namespace ELearningIskoop.Users.Application.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserManager _userManager;
        private readonly IPasswordResetRepository _passwordResetRepository;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;

        public ForgotPasswordCommandHandler(IUserRepository userRepository,IPasswordResetRepository passwordResetRepository,IUserManager userManager,ILogger<ForgotPasswordCommandHandler> logger)
        {
            _userRepository = userRepository;
            _passwordResetRepository = passwordResetRepository;
            _userManager = userManager;
            _logger = logger;
        }

        public  async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(new Email(request.Email), cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Forgot password requested for non-existing email: {Email}", request.Email);

            }

            // Token üret
            var token = GenerateResetToken();

            // Token kaydet
            var resetRequest = new PasswordResetToken(
                user.ObjectId,
                token,
                DateTime.UtcNow.AddMinutes(5)
            );

           

            var result = await _userManager.ForgotPasswordAsync(resetRequest);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Verify email failed for user: {Email}", request.Email);
                throw new BusinessRuleViolationException("VERIFY_EMAIL_FAILED", result.Error ?? "Verify email failed");
            }
            _logger.LogInformation("Verify email successfully for user: {Email}", request.Email);


            
             
           
        }

        private string GenerateResetToken()
        {
            var bytes = new byte[4];
            RandomNumberGenerator.Fill(bytes);
            var number = BitConverter.ToUInt32(bytes, 0) % 1_000_000;
            return number.ToString("D6");
        }
    }
}
