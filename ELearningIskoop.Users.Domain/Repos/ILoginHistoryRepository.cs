using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Users.Domain.Aggregates;
using ELearningIskoop.Users.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Domain.Repos
{
    public interface ILoginHistoryRepository : IRepository<LoginHistory>
    {
        Task RecordSuccessfulLoginAsync(int userId, string ipAddress, DateTime loginTime);
        Task RecordFailedLoginAsync(int userId, string ipAddress, DateTime attemptTime);
    }
}
