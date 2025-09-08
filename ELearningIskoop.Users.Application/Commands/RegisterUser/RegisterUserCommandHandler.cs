using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.Shared.Domain.ValueObjects;
using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Application.Mappers;
using ELearningIskoop.Users.Application.Services;
using Microsoft.Extensions.Logging;

namespace ELearningIskoop.Users.Application.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, UserDTO>
    {

        private readonly IUserManager _userManager;
        private readonly ILogger<RegisterUserCommandHandler> _logger;

        public RegisterUserCommandHandler(IUserManager userManager,ILogger<RegisterUserCommandHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<UserDTO> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Registering new user with email: {Email}", request.Email);
            var createdUserDto = new CreateUserDto(new Email(request.Email),
                new PersonName(request.FirstName, request.LastName), request.Password);

            var result = await _userManager.CreateUserAsync(createdUserDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("User registration failed: {Error}", result.Error);
                throw new ApplicationException(result.Error ?? "User registration failed");
            }
            _logger.LogInformation("User registered successfully: {UserId}", result.Value.ObjectId);

            return UserMapper.ToDTO(result.Value);
        }
    }
}
