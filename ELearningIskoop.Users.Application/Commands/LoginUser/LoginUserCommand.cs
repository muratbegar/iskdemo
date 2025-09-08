using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.Users.Application.DTOs;

namespace ELearningIskoop.Users.Application.Commands.LoginUser
{
    public record LoginUserCommand : BaseCommand<LoginResponseDTO>
    {
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string? IpAddress { get; init; }
        public string? UserAgent { get; init; }
    }

    public record LoginResponseDTO
    {
        public UserDTO User { get; init; } = null;
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
    }
}
