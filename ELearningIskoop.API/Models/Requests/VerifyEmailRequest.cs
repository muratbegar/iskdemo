namespace ELearningIskoop.API.Models.Requests
{
    public class VerifyEmailRequest
    {
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
