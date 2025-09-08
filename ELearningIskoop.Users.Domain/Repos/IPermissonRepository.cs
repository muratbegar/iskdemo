using ELearningIskoop.Users.Domain.Entities;
using ELearningIskoop.Users.Domain.ValueObjects.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = ELearningIskoop.Users.Domain.ValueObjects.Permissions.Action;

namespace ELearningIskoop.Users.Domain.Repos
{
    public interface IPermissonRepository
    {
        Task<Permission> GetByIdAsync(int id);
        Task<List<Permission>> GetByRoleIdAsync(int roleId);
        Task<List<Permission>> GetActiveByRoleIdAsync(int roleId);
        Task<Permission> GetByRoleAndPermissionAsync(int roleId, PermissionValue permissionValue);
        Task<List<Permission>> GetByResourceAsync(Resource resource);
        Task<List<Permission>> GetByActionAsync(Action action);
        Task<List<Permission>> GetByScopeAsync(Scope scope);
        Task<List<Permission>> SearchAsync(Resource? resource = null, Action? action = null, Scope? scope = null);
        Task<Permission> AddAsync(Permission permission);
        void Update(Permission permission);
        void Delete(Permission permission);
        Task<bool> ExistsAsync(int roleId, PermissionValue permissionValue);
    }
}
