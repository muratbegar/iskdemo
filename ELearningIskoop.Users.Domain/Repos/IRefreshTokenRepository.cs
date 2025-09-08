using ELearningIskoop.Users.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Domain.Repos
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> AddAsync(RefreshToken entity, CancellationToken cancellationToken = default);
    }
}
