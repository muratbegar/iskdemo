using ELearningIskoop.Users.Domain.EnumList;

namespace ELearningIskoop.API.Models.Requests
{
    public class SearchUsersRequest
    {
        public string? SearchTerm { get; set; }
        public string? Role { get; set; }
        public UserStatus? Status { get; set; }
        public bool? IsEmailVerified { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = true;
    }
}
