using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Application.DTOs.Role;
using MediatR;

namespace ELearningIskoop.Users.Application.Queries.GetRoleById
{
    public class GetRoleIdQuery : IRequest<RoleDto>
    {
        public int RoleId { get; }

        public GetRoleIdQuery(int roleId)
        {
            RoleId = roleId;
        }
    }
}
