using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.BuildingBlocks.Application.Results;
using ELearningIskoop.Users.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Commands.DeleteRole
{
    public record DeleteRoleCommand : BaseCommand<Result>
    {
        public int RoleId { get; set; }
    }
}
