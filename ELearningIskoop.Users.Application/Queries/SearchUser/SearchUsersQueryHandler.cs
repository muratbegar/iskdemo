using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Application.Mappers;
using ELearningIskoop.Users.Application.Queries.GetUserById;
using ELearningIskoop.Users.Domain.Repos;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Queries.SearchUser
{
    public class SearchUsersQueryHandler : IQueryHandler<SearchUserQuery, PagedResult<UserDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetUserByIdQueryHandler> _logger;

        public SearchUsersQueryHandler(IUserRepository userRepository,ILogger<GetUserByIdQueryHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<PagedResult<UserDTO>> Handle(SearchUserQuery request, CancellationToken cancellationToken)
        {
            var (users,totalCount) = await _userRepository.SearchAsync(
                request.SearchTerm,
                request.PageNumber,
                request.PageSize,
                request.SortBy,
                request.SortDescending,
                cancellationToken);

            return new PagedResult<UserDTO>(
                users.Select(UserMapper.ToDTO).ToList(),
                totalCount,
                request.PageNumber,
                request.PageSize);
        }
    }
}
