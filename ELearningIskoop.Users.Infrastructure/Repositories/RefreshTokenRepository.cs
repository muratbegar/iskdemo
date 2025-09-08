using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Domain.Entities;
using ELearningIskoop.Users.Domain.Repos;
using ELearningIskoop.Users.Infrastructure.Persistence;

namespace ELearningIskoop.Users.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly UserDbContext _context;

        public RefreshTokenRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> AddAsync(RefreshToken entity, CancellationToken cancellationToken = default)
        {
           _context.RefreshTokens.Add(entity);
            return entity;
        }
    }
}
