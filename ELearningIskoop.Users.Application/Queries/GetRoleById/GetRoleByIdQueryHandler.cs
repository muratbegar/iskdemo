using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Application.DTOs.Role;
using ELearningIskoop.Users.Domain.Entities;
using ELearningIskoop.Users.Domain.Repos;
using MediatR;

namespace ELearningIskoop.Users.Application.Queries.GetRoleById
{
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleIdQuery,RoleDto>
    {
        
        private readonly IRoleRepository _roleRepository;

        public GetRoleByIdQueryHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<RoleDto> Handle(GetRoleIdQuery request, CancellationToken cancellationToken)
        {
           var role = await _roleRepository.GetByIdAsync(request.RoleId);

           if (role == null || role.IsDeleted)
           {
               return null;
           }

              return new RoleDto
              {
                Id = role.ObjectId,
                RoleName = role.Name,
                Description = role.Description,
                CreatedAt = role.CreatedAt,
                UpdatedAt = role.UpdatedAt,
              };

        }
    }
}
