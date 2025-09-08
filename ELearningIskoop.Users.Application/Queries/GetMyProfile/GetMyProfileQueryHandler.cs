using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Domain.Repos;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Queries.GetMyProfile
{
    public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, UserDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetMyProfileQueryHandler(IUserRepository userRepository,IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<UserDTO> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            var idClaim = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            

            var userId = int.Parse(idClaim);
            var user = await _userRepository.GetByIdAsync(userId);

            if(user == null)
                throw new KeyNotFoundException("User not found");

            return new UserDTO
            {
                Id = user.ObjectId,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.Name.FirstName,
                LastName = user.Name.LastName,
                FullName = user.Name.FirstName + user.Name.LastName,
                Status = user.Status.ToString(),
                IsEmailVerified = user.IsMailVerified,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Roles = user.UserRoles.Select(r => r.Role.Name).ToList()
            };


        }
    }
}
