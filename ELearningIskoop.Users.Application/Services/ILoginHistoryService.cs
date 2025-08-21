using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Services
{
    public interface ILoginHistoryService
    {
        Task RecordSuccessfulLoginAsync(int userId, string ipAddress, DateTime loginTime);
        Task RecordFailedLoginAsync(int userId, string ipAddress, DateTime attemptTime);
    }
}
