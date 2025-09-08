using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Application.Services
{
    public interface ICurrentUserService
    {
        string GetCurrentUserId();
        string GetCurrentUserName();
        string GetCurrentUserEmail();
        bool IsAuthenticated();
        bool IsInRole(string role);
        IEnumerable<string> GetUserRoles();
    }
}
