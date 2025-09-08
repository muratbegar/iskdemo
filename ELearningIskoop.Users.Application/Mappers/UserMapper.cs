using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Domain.Aggregates;

namespace ELearningIskoop.Users.Application.Mappers
{
    public static class UserMapper
    {
        public static UserDTO ToDTO(User user)
        {
            return new UserDTO
            {
                Id = user.ObjectId,
                Email = user.Email.Value,
                FirstName = user.Name.FirstName,
                LastName = user.Name.LastName,
                FullName = user.Name.FirstName + user.Name.LastName,
                Username = user.Username,
                Status = user.Status.ToString(),
                IsEmailVerified = user.IsMailVerified,
                LastLoginAt = user.LastLoginAt,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Roles = user.UserRoles.Select(x=>x.Role.Name).ToList(),
                CreatedAt = user.CreatedAt,
            };
        }
    }
}
