using ELearningIskoop.BuildingBlocks.Application.Results;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Shared.Domain.Enums;
using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Domain.Aggregates;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Shared.Domain.ValueObjects;
using ELearningIskoop.Users.Domain.Entities;
using ELearningIskoop.Users.Domain.Repos;

namespace ELearningIskoop.Users.Application.Services
{
    public class UserManager : IUserManager
    {

        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserEmailVerificationRepository _userEmailVerificationRepository;
        private readonly IPasswordResetRepository _passwordResetRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserManager> _logger;

        public UserManager(IUserRepository userRepository, IRoleRepository roleRepository, IUnitOfWork unitOfWork, IUserEmailVerificationRepository userEmailVerificationRepository,IPasswordResetRepository passwordResetRepository, ILogger<UserManager> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userEmailVerificationRepository = userEmailVerificationRepository;
            _passwordResetRepository = passwordResetRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<User>> CreateUserAsync(CreateUserDto dto)
        {
            try
            {
                //Email benzersizlik kontrolü
                var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
                if (existingUser != null)
                {
                    return Result.Failure<User>("Email is already registered");
                }

                //Kullanıcı oluştur
                var user = User.Create(dto.Email, dto.Name, dto.Password);

                //Default role ata
                var defaultRole = await _roleRepository.GetByNameAsync(enUserRole.Student.ToString());
                if (defaultRole != null)
                {
                    user.AssignRole(defaultRole);
                }

                //Doğrulama kodu oluştur ve ata
                var verificationCode = Random.Shared.Next(100000, 999999).ToString();
                var verification =
                    new UserEmailVerification(user.Email, verificationCode, DateTime.UtcNow.AddMinutes(3));
                await _userEmailVerificationRepository.AddAsync(verification);




                await _userRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("User created: {UserId} - {Email}", user.ObjectId, user.Email.Value);

                return Result.Success(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {Email}", dto.Email.Value);
                return Result.Failure<User>("An error occurred while creating user");
            }
        }

        public async Task<Result<User>> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdWithRolesAsync(userId);
                return user != null ? Result.Success(user) : Result.Failure<User>("User not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "User not found");
                return Result.Failure<User>("An error occurred while creating user");
            }

        }

        public async Task<Result<User>> GetUserByEmailAsync(Email email)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                return user != null ? Result.Success(user) : Result.Failure<User>("User not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "User not found");
                return Result.Failure<User>("An error occurred while creating user");
            }


        }

        public async Task<Result<User>> AuthenticateAsync(Email email, string password, string ipAddress)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    // Timing attack prevention
                    await Task.Delay(Random.Shared.Next(100, 500));
                    return Result.Failure<User>("Invalid credentials");

                }

                var loginResult = user.AttemptLogin(password, ipAddress);
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                if (!loginResult.IsSuccess)
                {
                    return Result.Failure<User>(loginResult.Error ?? "Authentication failed");
                }

                return Result.Success(user);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "User Invalid credentials");
                return Result.Failure<User>("An error occurred while creating user");
            }

        }

        public async Task<Result> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure("User not found");
            }

            try
            {
                user.ChangePassword(currentPassword, newPassword);
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success();
            }
            catch (BusinessRuleViolationException ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result> ResetPasswordAsync(Email email, string newPassword, string token)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return Result.Failure("User not found");
            }
            
            var resetPassword = await _passwordResetRepository.GetByUserIdAsync(user.ObjectId);

            if (resetPassword.Token != token)
            {
                return Result.Failure("Şifre resetlenemedi");
            }

            user.ResetPassword(newPassword,user.ObjectId);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> VerifyEmailAsync(int userId,string code)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure("User not found");
            }

            try
            {

                var verification = await _userEmailVerificationRepository.GetByUserMailAsync(user.Email);
                if (verification == null && !verification.IsValid(code))
                {
                    return Result.Failure("Invalid or expired verification code");
                }

                if (verification.Code != code)
                {
                    return Result.Failure("Invalid or expired verification code");
                }
                
                
                if (verification.Code == code)
                {
                    user.VerifyEmail(userId);
                    verification.MarkAsUsed();
                    _userEmailVerificationRepository.UpdateAsync(verification);
                }
                
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result> ForgotPasswordAsync(PasswordResetToken tokenDto)
        {
          
            try
            {

                await _passwordResetRepository.AddAsync(tokenDto,CancellationToken.None);
                await _unitOfWork.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result> AssignRoleAsync(int userId, string roleName)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId);
            if (user == null)
            {
                return Result.Failure("User not found");
            }

            var role = await _roleRepository.GetByNameAsync(roleName);
            if (role == null)
            {
                return Result.Failure("Role not found");
            }

            user.AssignRole(role);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> RemoveRoleAsync(int userId, string roleName)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId);
            if (user == null)
            {
                return Result.Failure("User not found");
            }

            var role = await _roleRepository.GetByNameAsync(roleName);
            if (role == null)
            {
                return Result.Failure("Role not found");
            }

            user.RemoveRule(role.ObjectId);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<bool> IsInRoleAsync(int userId, string roleName)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId);
            return user?.HasRole(roleName) ?? false;
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId);
            if (user == null)
            {
                return Enumerable.Empty<string>();
            }

            return user.UserRoles.Select(ur => ur.Role.Name);
        }
    }

}
