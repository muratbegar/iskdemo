using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Domain.ValueObjects.Permissions;

namespace ELearningIskoop.Users.Domain.Services
{
    public interface IPermissionDomainService
    {
        //VALIDATION METHODS

        Task ValidatePermissionAsync(PermissionValue permissionValue);
        Task ValidatePermissionAssigmentAsync(int roleId,PermissionValue permissionValue);
        Task<bool> CanAssignPermissionToRoleAsync(int roleId, PermissionValue permissionValue);

        //BUSINESS RULE CHECKS
        Task<bool> HasPermissionConflictAsync(int roleId, PermissionValue permissionValue);
        Task<List<PermissionValue>> GetConflictingPermissionAsync(int roleId, PermissionValue permissionValue);
        Task<bool> IsSystemPermissionAsync(PermissionValue permissionValue);
        Task ValidateSystemPermissionAccessAsync(string currentUserId, PermissionValue permissionValue);

        //PERMISSION ANALYSIS
        Task<List<PermissionValue>> GetRedundantPermissionsAsync(int roleId);
        Task<List<PermissionValue>> OptimizeRolePermissionsAsync(int roleId);
        Task<PermissionAnalysisResult> AnalyzeRolePermissionsAsync(int roleId);
    }

    public class PermissionAnalysisResult
    {
        public int TotalPermissions { get; set; }
        public int ActivePermissions { get; set; }
        public int EffectivePermissions { get; set; }
        public List<PermissionValue> RedundantPermissions { get; set; } = new();
        public List<PermissionValue> ConflictingPermissions { get; set; } = new();
        public List<PermissionValue> SystemPermissions { get; set; } = new();
        public Dictionary<string, int> PermissionsByResource { get; set; } = new();
        public Dictionary<string, int> PermissionsByAction { get; set; } = new();
        public Dictionary<string, int> PermissionsByScope { get; set; } = new();
    }
}
