using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Domain.Repos;
using FluentValidation;

namespace ELearningIskoop.Users.Application.Commands.CreateRole
{
    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        private readonly IRoleRepository _roleRepository;

        public CreateRoleCommandValidator(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;


            RuleFor(x => x.RoleName)
                .NotEmpty()
                .WithMessage("Role name is required")
                .MaximumLength(50)
                .WithMessage("Role name cannot exceed 50 characters")
                .MustAsync(BeUniqueRoleName)
                .WithMessage("Role with this name already exists");

            RuleFor(x => x.Description)
                .MaximumLength(250)
                .WithMessage("Description cannot exceed 250 characters");

            // Business rule validation
            RuleFor(x => x.RoleName)
                .Must(NotBeSuperAdmin)
                .WithMessage("SuperAdmin role cannot be created manually")
                .When(x => !string.IsNullOrWhiteSpace(x.RoleName));

        }
        private async Task<bool> BeUniqueRoleName(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
                return true;

            return !await _roleRepository.NameExistsAsync(name);
        }

        private bool NotBeSuperAdmin(string name)
        {
            return !name.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
