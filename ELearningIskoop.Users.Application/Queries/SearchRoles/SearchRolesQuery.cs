using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Application.DTOs.Role;
using MediatR;

namespace ELearningIskoop.Users.Application.Queries.SearchRoles
{
    public class SearchRolesQuery : IRequest<PagedRoleResult>
    {
        public string SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedDate";
        public bool SortDescending { get; set; } = false;

        public SearchRolesQuery()
        {
        }

        public SearchRolesQuery(string searchTerm, int page = 1, int pageSize = 10)
        {
            SearchTerm = searchTerm;
            Page = page > 0 ? page : 1;
            PageSize = pageSize > 0 ? pageSize : 10;
        }
    }
}
