using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Domain.Entities;
using ELearningIskoop.Users.Domain.Repos;
using ELearningIskoop.Users.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ELearningIskoop.Users.Infrastructure.Repositories
{
    public class PasswordResetRepository : IPasswordResetRepository
    {

        private readonly UserDbContext _context;

        public PasswordResetRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<PasswordResetToken?> GetByUserIdAsync(int userId)
        {
            return await _context.PasswordResetTokens.FirstOrDefaultAsync(prt => prt.UserId == userId);
        }

        public async Task AddAsync(PasswordResetToken resetToken, CancellationToken cancellationToken)
        {
            await _context.PasswordResetTokens.AddAsync(resetToken);
        }

        public void UpdateAsync(PasswordResetToken resetToken)
        {
          _context.PasswordResetTokens.Update(resetToken);
        }
    }
}
