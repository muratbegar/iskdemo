using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.BuildingBlocks.Application.Results;
using ELearningIskoop.Users.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Commands.UpdateRole
{
    public record UpdateRoleCommand : BaseCommand<Result>
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
