using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.Users.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Application.Results;
using ELearningIskoop.Users.Domain.Entities;

namespace ELearningIskoop.Users.Application.Commands.CreateRole
{
    public record CreateRoleCommand : BaseCommand<Result<Role>>
    {
        public string RoleName { get; set; }
        public string Description { get; set; }
    }
    

}
