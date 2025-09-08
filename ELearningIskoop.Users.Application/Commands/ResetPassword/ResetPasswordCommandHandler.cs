using ELearningIskoop.Shared.Domain.ValueObjects;
using ELearningIskoop.Users.Application.Services;
using ELearningIskoop.Users.Domain.Repos;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
    {

        private readonly IUserManager _userManager;
        private readonly ILogger<ResetPasswordCommandHandler> _logger;

        public ResetPasswordCommandHandler(IUserManager userManager,ILogger<ResetPasswordCommandHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var email = new Email(request.Email);
            var result = await _userManager.ResetPasswordAsync(request.Email, request.NewPassword, request.Token);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Login failed for email: {Email}. Error: {Error}", request.Email, result.Error);
                throw new ApplicationException(result.Error ?? "Login failed");
            }
        }
    }
}
