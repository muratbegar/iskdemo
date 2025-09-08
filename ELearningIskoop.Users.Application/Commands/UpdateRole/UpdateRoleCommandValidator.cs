using ELearningIskoop.Users.Domain.Repos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Commands.UpdateRole
{
    public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
    {
        private readonly IRoleRepository _roleRepository;
        public UpdateRoleCommandValidator(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;

            RuleFor(x => x.RoleId)
                .NotEmpty()
                .WithMessage("Role ID is required")
                .GreaterThan(0)
                .WithMessage("Role ID must be greater than 0")
                .MustAsync(RoleExists)
                .WithMessage("Role not found");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Role name is required")
                .MaximumLength(50)
                .WithMessage("Role name cannot exceed 50 characters")
                .MustAsync((command, name, cancellationToken) => BeUniqueRoleName(name, cancellationToken))
                .WithMessage("Role with this name already exists")
                .When(x => x.RoleId > 0); // Sadece valid RoleId olduğunda unique check yap

            RuleFor(x => x.Description)
                .MaximumLength(250)
                .WithMessage("Description cannot exceed 250 characters");

            // Business rule validations
            RuleFor(x => x)
                .MustAsync(NotBeSystemRole)
                .WithMessage("System roles cannot be modified")
                .When(x => x.RoleId > 0);
        }

        private async Task<bool> RoleExists(int roleId, CancellationToken cancellationToken)
        {
            if (await RoleExists(5, cancellationToken))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> BeUniqueRoleName(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
                return true;

            return !await _roleRepository.NameExistsAsync(name);
        }

        private async Task<bool> NotBeSystemRole(UpdateRoleCommand command, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(command.RoleId);
            if (role == null)
                return true; // Role yoksa sistem role değil

            return !role.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
                   !role.Name.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
