using ELearningIskoop.BuildingBlocks.Application.Behaviors;
using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.Users.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Queries.GetMyProfile
{
    public record GetMyProfileQuery : BaseQuery<UserDTO>, ICacheableRequest
    {
        public string CacheKey => $"UserProfile_{RequestedBy}";
        public TimeSpan CacheDuration => TimeSpan.FromMinutes(30);
    }
}
