using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Users.Domain.Aggregates;
using ELearningIskoop.Users.Domain.Entities;
using ELearningIskoop.Users.Domain.Repos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Services
{
    public class TokenService : ITokenService
    {

        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _secretKey;
        private readonly int _accessTokenExpirationMinutes;
        private readonly int _refreshTokenExpirationDays;

        public TokenService(IConfiguration configuration,IUserRepository userRepository,IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _secretKey = _configuration["ELearningIskoop:Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            _accessTokenExpirationMinutes = int.Parse(
                _configuration.GetSection("Jwt:AccessTokenExpirationMinutes").Value ?? "60");
            _refreshTokenExpirationDays = int.Parse(
                _configuration.GetSection("Jwt:RefreshTokenExpirationDays").Value ?? "7");
        }

        public async Task<TokenResult> GenerateTokenAsync(User user, IEnumerable<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var expiresAt = DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.ObjectId.ToString()),
                new Claim(ClaimTypes.Email, user.Email.Value),
                new Claim(ClaimTypes.Name, user.Name.FirstName+user.Name.LastName),
                new Claim("username", user.Username),
                new Claim("email_verified", user.IsMailVerified.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            };
            // Add role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["ELearningIskoop:Jwt:Issuer"],
                Audience = _configuration["ELearningIskoop:Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            // Generate refresh token
            var refreshToken = GenerateRefreshToken();

            //Save refresh token to user
           var refreshTokenEntity = RefreshToken.Create(
               user.ObjectId,
               refreshToken,
               "System",
               _refreshTokenExpirationDays
           );
            // This would need to be added to User aggregate
            // For now, we'll assume this is handled in the repository

            return new TokenResult
            {
                AccessToken = accessToken,
                RefreshToken = "refreshToken",
                ExpiresAt = expiresAt,
                TokenType = "Bearer"
            };
        }

        public async Task<TokenValidationResult> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["ELearningIskoop:Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["ELearningIskoop:Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value);

                return new TokenValidationResult
                {
                    IsValid = true,
                    UserId = int.TryParse(userIdClaim, out var userId) ? userId : null,
                    Roles = roles
                };
            }
            catch (SecurityTokenException ex)
            {
                return new TokenValidationResult
                {
                    IsValid = false,
                    Error = ex.Message
                };
            }
        }

        public async Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);

            if (user != null)
            {
                return new RefreshTokenResult
                {
                    IsSuccess = false,
                    Error = "Invalid refresh token"
                };
            }

            var token = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);

            if (token == null || !token.IsActive)
            {
                return new RefreshTokenResult
                {
                    IsSuccess = false,
                    Error = "Refresh token is expired or revoked"
                };
            }

            // Revoke old refresh token
            user.RevokeRefreshToken(refreshToken, "Token refreshed", "System");

            // Generate new tokens
            var roles = user.UserRoles.Select(ur => ur.Role.Name);
            var newTokenResult = await GenerateTokenAsync(user, roles);

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new RefreshTokenResult
            {
                IsSuccess = true,
                AccessToken = newTokenResult.AccessToken,
                NewRefreshToken = newTokenResult.RefreshToken,
                ExpiresAt = newTokenResult.ExpiresAt
            };
        }

        public async Task RevokeTokenAsync(string refreshToken, string reason)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);

            if (user != null)
            {
                user.RevokeRefreshToken(refreshToken, reason, "System");
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
