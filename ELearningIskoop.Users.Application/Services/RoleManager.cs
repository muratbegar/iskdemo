using ELearningIskoop.BuildingBlocks.Application.Results;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Domain.Entities;
using ELearningIskoop.Users.Domain.Repos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ELearningIskoop.Users.Application.Services
{
    public class RoleManager : IRoleManager
    {

        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RoleManager> _logger;

        public RoleManager(IRoleRepository roleRepository,IUnitOfWork unitOfWork,ILogger<RoleManager> logger)
        {
            _logger = logger;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> IsRoleNameUniqueAsync(string roleName, int? excludeRoleId = null)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return false;




            var res = !await _roleRepository.NameExistsAsync(roleName);
            return Result.Success(res);
        }

        public async Task<Result> ValidateCreateRoleAsync(string roleName)
        {
            if(string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Rol adı boş olamaz", nameof(roleName));

            var resUnique = await IsRoleNameUniqueAsync(roleName);

            if (!resUnique.Value)
                throw new InvalidOperationException("Rol adı zaten mevcut");
            if (roleName.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("SuperAdmin role cannot be created manually");

            return Result.Success();
        }

        public async Task<Result> ValidateUpdateRoleAsync(int roleId, string newRoleName)
        {
            // Business Rule 1: Role var mı kontrol
            var existingRole = await _roleRepository.GetByIdAsync(roleId);
            if (existingRole == null)
                throw new InvalidOperationException($"Role with ID {roleId} not found");

            // Business Rule 2: Silinen role güncellenemez
            if (existingRole.IsDeleted)
                throw new InvalidOperationException("Cannot update deleted role");

            // Business Rule 3: Name değişiyorsa unique kontrolü
            if (!existingRole.Name.Equals(newRoleName, StringComparison.OrdinalIgnoreCase))
            {
                var resUnique = await IsRoleNameUniqueAsync(newRoleName);
                if (!resUnique.Value)
                    throw new InvalidOperationException("Rol adı zaten mevcut");
            }

            // Business Rule 4: System role'ler korumalı (örnek)
            if (existingRole.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                existingRole.Name.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("System roles cannot be modified");
            }

            return Result.Success();
        }

        public async Task<Result> ValidateRoleDeletionAsync(int roleId)
        {
            // Business Rule 1: Role var mı kontrol
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
                throw new InvalidOperationException($"Role with ID {roleId} not found");

            // Business Rule 2: Zaten silinmiş mi
            if (role.IsDeleted)
                throw new InvalidOperationException("Role is already deleted");

            // Business Rule 3: System role'ler silinemez
            if (role.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                role.Name.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("System roles cannot be deleted");
            }

            return Result.Success();

        }

        public async Task<Result<Role>> CreateRoleAsync(string roleName, string description, IEnumerable<string> permissions)
        {
            // Validation'ları çalıştır
            await ValidateCreateRoleAsync(roleName);

            var role = Role.Create(roleName, description);
            // Entity değişti, EF change tracking takip ediyor
            // UnitOfWork.SaveChanges() handler'da yapılacak
            return Result.Success(role);
        }

        public async Task<Result> UpdateRoleAsync(int roleId, string newName, string newDescription)
        {
            await ValidateUpdateRoleAsync(roleId, newName);
            var role = await _roleRepository.GetByIdAsync(roleId);

            // Güncellemeleri yap
            if (!role.Name.Equals(newName, StringComparison.OrdinalIgnoreCase))
            {
                role.UpdateName(newName);
                Result.Failure("");
            }

            if (!string.Equals(role.Description, newDescription))
            {
                role.UpdateDescription(newDescription);
                Result.Failure("");
            }
            // Entity değişti, EF change tracking takip ediyor
            // UnitOfWork.SaveChanges() handler'da yapılacak

            return Result.Success(role);
        }

        public async Task<Result> DeleteRoleAsync(int roleId)
        {
            // Validation'ları çalıştır
            await ValidateRoleDeletionAsync(roleId);

            // Role'u getir ve soft delete yap
            var role = await _roleRepository.GetByIdAsync(roleId);
            role.MarkAsDeleted();

            return Result.Success();
            // Entity değişti, EF change tracking takip ediyor
            // UnitOfWork.SaveChanges() handler'da yapılacak
        }
    }
}
