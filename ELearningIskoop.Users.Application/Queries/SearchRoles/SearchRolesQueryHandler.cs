using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Application.DTOs.Role;
using ELearningIskoop.Users.Domain.Repos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Queries.SearchRoles
{
    public class SearchRolesQueryHandler : IRequestHandler<SearchRolesQuery, PagedRoleResult>
    {
        private readonly IRoleRepository _roleRepository;

        public SearchRolesQueryHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        }


        public async Task<PagedRoleResult> Handle(SearchRolesQuery request, CancellationToken cancellationToken)
        {
            // Validate pagination parameters
            var page = Math.Max(1, request.Page);
            var pageSize = Math.Min(100, Math.Max(1, request.PageSize));

            // Search roles
            var searchResults = string.IsNullOrWhiteSpace(request.SearchTerm)
                ? await _roleRepository.GetAllAsync()
                : await _roleRepository.SearchAsync(request.SearchTerm.Trim());

            // Apply sorting
            var sortedResults = request.SortBy?.ToLower() switch
            {
                "createdat" => request.SortDescending
                    ? searchResults.OrderByDescending(r => r.CreatedAt)
                    : searchResults.OrderBy(r => r.CreatedAt),
                _ => request.SortDescending
                    ? searchResults.OrderByDescending(r => r.Name)
                    : searchResults.OrderBy(r => r.Name)
            };

            // Apply pagination
            var totalCount = sortedResults.Count();
            var pagedResults = sortedResults
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(role => new RoleListDto
                {
                    Id = role.ObjectId,
                    Name = role.Name,
                    Description = role.Description,
                    CreatedAt = role.CreatedAt
                })
                .ToList();

            return new PagedRoleResult
            {
                Roles = pagedResults,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
