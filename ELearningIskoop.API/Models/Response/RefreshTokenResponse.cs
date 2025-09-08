namespace ELearningIskoop.API.Models.Response
{
    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
