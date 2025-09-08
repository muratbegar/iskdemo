using ELearningIskoop.BuildingBlocks.Application.Results;
using ELearningIskoop.Shared.Domain.ValueObjects;
using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Domain.Aggregates;
using ELearningIskoop.Users.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Services
{
    public interface IUserManager
    {
        Task<Result<User>> CreateUserAsync(CreateUserDto dto);
        Task<Result<User>> GetUserByIdAsync(int userId);
        Task<Result<User>> GetUserByEmailAsync(Email email);
        Task<Result<User>> AuthenticateAsync(Email email, string password, string ipAddress);

        Task<Result> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<Result> ResetPasswordAsync(Email email, string newPassword,string token);
        Task<Result> VerifyEmailAsync(int userId,string code);

        Task<Result> ForgotPasswordAsync(PasswordResetToken dto);
        Task<Result> AssignRoleAsync(int userId, string roleName);
        Task<Result> RemoveRoleAsync(int userId, string roleName);
        Task<bool> IsInRoleAsync(int userId, string roleName);
        Task<IEnumerable<string>> GetUserRolesAsync(int userId);
    }

}
