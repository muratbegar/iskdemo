using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Application.DTOs.Role;
using ELearningIskoop.Users.Domain.Repos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Queries.GetAllRoles
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery,PagedResult<RoleListDto>>
    {
        private readonly IRoleRepository _roleRepository;

        public GetAllRolesQueryHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        }

        public async Task<PagedResult<RoleListDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            // Validate pagination parameters
            var page = Math.Max(1, request.PageNumber);
            var pageSize = Math.Min(100, Math.Max(1, request.PageSize)); // Max 100 items per page

            // Get all roles (repository'de pagination henüz yok, ileride eklenebilir)
            var allRoles = await _roleRepository.GetAllAsync();

            // Apply sorting
            var sortedRoles = request.SortBy?.ToLower() switch
            {
                "createdat" => request.SortDescending
                    ? allRoles.OrderByDescending(r => r.CreatedAt)
                    : allRoles.OrderBy(r => r.CreatedAt),
                _ => request.SortDescending
                    ? allRoles.OrderByDescending(r => r.Name)
                    : allRoles.OrderBy(r => r.Name)
            };
            // Apply pagination
            var totalCount = sortedRoles.Count();
            var pagedRoles = sortedRoles
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(role => new RoleListDto
                {
                    Id = role.ObjectId,
                    Name = role.Name,
                    Description = role.Description,
                    CreatedAt = role.CreatedAt
                }).ToList();

            return new PagedResult<RoleListDto>(pagedRoles, totalCount, page, pageSize);
        }
    }
}
