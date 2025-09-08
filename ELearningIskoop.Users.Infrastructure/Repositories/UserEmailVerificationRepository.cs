using ELearningIskoop.Users.Domain.Entities;
using ELearningIskoop.Users.Domain.Repos;
using ELearningIskoop.Users.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ELearningIskoop.Users.Infrastructure.Repositories
{
    public class UserEmailVerificationRepository : IUserEmailVerificationRepository
    {
        private readonly UserDbContext _context;

        public UserEmailVerificationRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<UserEmailVerification> GetByUserMailAsync(string userEmail)
        {
           return await _context.UserEmailVerifications
                .FirstOrDefaultAsync(uev => uev.UserMail == userEmail);
        }

        public async Task AddAsync(UserEmailVerification entity)
        {
            await _context.UserEmailVerifications.AddAsync(entity);
        }

        public void UpdateAsync(UserEmailVerification entity)
        {
            _context.UserEmailVerifications.Update(entity);
        }
    }
}
