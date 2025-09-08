using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Domain.Entities;

namespace ELearningIskoop.Users.Domain.Repos
{
    public interface IPasswordResetRepository
    {
        Task<PasswordResetToken> GetByUserIdAsync(int userId);
        Task AddAsync(PasswordResetToken resetToken,CancellationToken cancellationToken);
        void UpdateAsync(PasswordResetToken resetToken);
    }
}
