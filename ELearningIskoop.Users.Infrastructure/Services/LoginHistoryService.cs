using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Application.Services;
using ELearningIskoop.Users.Domain.Entities;
using ELearningIskoop.Users.Domain.Repos;

namespace ELearningIskoop.Users.Infrastructure.Services
{
    public class LoginHistoryService : ILoginHistoryService
    {

        private readonly ILoginHistoryRepository _loginHistoryRepository;

        public LoginHistoryService(ILoginHistoryRepository loginHistoryRepository)
        {
            _loginHistoryRepository = loginHistoryRepository;
        }

        public async Task RecordSuccessfulLoginAsync(int userId, string ipAddress, DateTime loginTime)
        {
            var loginHistory= new LoginHistory
            {
                UserId = userId,
                IpAddress = ipAddress,
                LoginTime = loginTime,
                IsSuccessful = true,
                
            };

            await _loginHistoryRepository.AddAsync(loginHistory);
            
        }

        public Task RecordFailedLoginAsync(int userId, string ipAddress, DateTime attemptTime)
        {
            throw new NotImplementedException();
        }
    }
}
