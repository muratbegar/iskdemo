using ELearningIskoop.BuildingBlocks.Application.Behaviors;
using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.Users.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Queries.SearchUser
{
    public record SearchUserQuery : BaseQuery<PagedResult<UserDTO>>, ICacheableRequest
    {
       
        public string? SearchTerm { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public string SortBy { get; init; } = string.Empty;
        public bool SortDescending { get; init; } = false;
        public string CacheKey => $"UserProfile_{RequestedBy}";
        public TimeSpan CacheDuration => TimeSpan.FromMinutes(30);
    }
    
}
