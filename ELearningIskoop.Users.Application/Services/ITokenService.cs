using ELearningIskoop.Users.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Services
{
    public interface ITokenService
    {
        Task<TokenResult> GenerateTokenAsync(User user, IEnumerable<string> roles);
        Task<TokenValidationResult> ValidateTokenAsync(string token);
        Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken);
        Task RevokeTokenAsync(string refreshToken, string reason);
    }

    public class TokenResult
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string TokenType { get; set; } = "Bearer";
    }

    public class TokenValidationResult
    {
        public bool IsValid { get; set; }
        public int? UserId { get; set; }
        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
        public string? Error { get; set; }
    }

    public class RefreshTokenResult
    {
        public bool IsSuccess { get; set; }
        public string? AccessToken { get; set; }
        public string? NewRefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? Error { get; set; }
    }
}
