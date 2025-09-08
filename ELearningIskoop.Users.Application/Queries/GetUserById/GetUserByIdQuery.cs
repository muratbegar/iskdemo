using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Application.Behaviors;
using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.Users.Application.DTOs;

namespace ELearningIskoop.Users.Application.Queries.GetUserById
{
    public record GetUserByIdQuery : BaseQuery<UserDTO>,ICacheableRequest
    {
        public int UserId { get; init; }

        public string CacheKey => $"GetUserByIdQuery_{UserId}";
        public TimeSpan CacheDuration => TimeSpan.FromMinutes(15);
    }
}
