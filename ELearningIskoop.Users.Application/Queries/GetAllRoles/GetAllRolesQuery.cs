using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Application.DTOs.Role;
using MediatR;

namespace ELearningIskoop.Users.Application.Queries.GetAllRoles
{
    public class GetAllRolesQuery : IRequest<PagedResult<RoleListDto>>
    {
        public int PageNumber { get; }
        public int PageSize { get; }
        public string SortBy { get; set; } = "CreatedAt"; // Name, CreatedAt
        public bool SortDescending { get; set; } = false;

        public GetAllRolesQuery()
        {
        }

        public GetAllRolesQuery(int pageNumber, int pageSize, string sortBy = "CreatedAt", bool sortDescending = false)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SortBy = sortBy ?? "Name";
            SortDescending = sortDescending;
        }
    }
}
