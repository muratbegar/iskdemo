using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Domain.Repos;
using FluentValidation;

namespace ELearningIskoop.Users.Application.Commands.DeleteRole
{
    public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
    {
        private readonly IRoleRepository _roleRepository;

        public DeleteRoleCommandValidator(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;

            RuleFor(x => x.RoleId)
                .NotEmpty()
                .WithMessage("Role ID is required")
                .GreaterThan(0)
                .WithMessage("Role ID must be greater than 0")
                .MustAsync(RoleExists)
                .WithMessage("Role not found")
                .MustAsync(NotBeSystemRole)
                .WithMessage("System roles cannot be deleted")
                .MustAsync(NotBeInUse)
                .WithMessage("Cannot delete role that is assigned to users");
        }

        private async Task<bool> RoleExists(int roleId, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            return role != null && !role.IsDeleted;
        }

        private async Task<bool> NotBeSystemRole(int roleId, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
                return true;

            return !role.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
                   !role.Name.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase);
        }

        private async Task<bool> NotBeInUse(int roleId, CancellationToken cancellationToken)
        {
            // TODO: UserRole repository ile kontrol edilecek
            // var userCount = await _userRoleRepository.GetUserCountByRoleAsync(roleId);
            // return userCount == 0;

            //user tablosundan o usera ait role kontrolü yapılacak

            return true; // Şimdilik true dön, UserRole implementasyonundan sonra düzenlenecek
        }
    }
}
