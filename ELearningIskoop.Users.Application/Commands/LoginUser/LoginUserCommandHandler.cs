using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.Users.Application.Mappers;
using ELearningIskoop.Users.Application.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Shared.Domain.ValueObjects;

namespace ELearningIskoop.Users.Application.Commands.LoginUser
{
    public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, LoginResponseDTO>
    {
        private readonly IUserManager _userManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<LoginUserCommandHandler> _logger;

        public LoginUserCommandHandler(IUserManager userManager,ITokenService tokenService,ILogger<LoginUserCommandHandler> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LoginResponseDTO> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var ipAddress = request.IpAddress ?? "Unknown";

            _logger.LogInformation("Login attempt for email: {Email} from IP: {IP}", request.Email, ipAddress);
            var email = new Email(request.Email);
            var result = await _userManager.AuthenticateAsync(email, request.Password, ipAddress);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Login failed for email: {Email}. Error: {Error}", request.Email, result.Error);
                throw new ApplicationException(result.Error ?? "Login failed");
            }

            var user = result.Value;
            var roles = await _userManager.GetUserRolesAsync(user.ObjectId);
            var tokenResult = await _tokenService.GenerateTokenAsync(user, roles);
            _logger.LogInformation("Login successful for user: {UserId}", user.ObjectId);

            return new LoginResponseDTO
            {
                User = UserMapper.ToDTO(user),
                AccessToken = tokenResult.AccessToken,
                RefreshToken = tokenResult.RefreshToken,
                ExpiresAt = tokenResult.ExpiresAt
            };
        }
    }
}
