using ELearningIskoop.BuildingBlocks.Application.Results;
using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Domain.Entities;

namespace ELearningIskoop.Users.Application.Services
{
    public interface IRoleManager
    {
        Task<Result<bool>> IsRoleNameUniqueAsync(string roleName, int? excludeRoleId = null);
        Task<Result> ValidateCreateRoleAsync(string roleName);
        Task<Result> ValidateUpdateRoleAsync(int roleId, string newRoleName);
        Task<Result> ValidateRoleDeletionAsync(int roleId);
        Task<Result<Role>> CreateRoleAsync(string roleName, string description, IEnumerable<string> permissions);
        Task<Result> UpdateRoleAsync(int roleId, string newName, string newDescription);
        Task<Result> DeleteRoleAsync(int roleId);
    }
}
