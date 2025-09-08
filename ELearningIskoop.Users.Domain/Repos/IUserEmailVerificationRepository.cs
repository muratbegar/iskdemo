using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Domain.Entities;

namespace ELearningIskoop.Users.Domain.Repos
{
    public interface IUserEmailVerificationRepository
    {
        Task<UserEmailVerification> GetByUserMailAsync(string userEmail);
        Task AddAsync(UserEmailVerification entity);
        void UpdateAsync(UserEmailVerification entity);
    }
}
