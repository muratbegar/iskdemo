using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Application.Mappers;
using ELearningIskoop.Users.Domain.Repos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Domain.Aggregates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace ELearningIskoop.Users.Application.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery,UserDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetUserByIdQueryHandler> _logger;

        public GetUserByIdQueryHandler(IUserRepository userRepository,IHttpContextAccessor httpContextAccessor,ILogger<GetUserByIdQueryHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserDTO> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            
            try
            {
                
                _logger.LogDebug("Getting user by ID: {UserId}", request.UserId);

                var user = await _userRepository.GetByIdWithRolesAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    throw new EntityNotFoundException("User", request.UserId);
                }
                return UserMapper.ToDTO(user);
            }
            catch (Exception e)
            {
                throw new EntityNotFoundException("User", request.UserId);
            }
           
        }
    }
}
