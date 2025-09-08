using ELearningIskoop.Users.Application.DTOs;

namespace ELearningIskoop.API.Models.Response
{
    public class LoginResponse
    {
        public UserDTO User { get; set; } = null!;
        public string AccessToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string TokenType { get; set; } = "Bearer";
    }
}
