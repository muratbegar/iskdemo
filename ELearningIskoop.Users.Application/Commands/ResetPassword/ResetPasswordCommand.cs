using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.Users.Application.Commands.LoginUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Shared.Domain.ValueObjects;

namespace ELearningIskoop.Users.Application.Commands.ResetPassword
{
    public class ResetPasswordCommand : BaseCommand , ICommand
    {
        public Email Email { get; init; }
        public string Token { get; init; } = string.Empty;
        public string NewPassword { get; init; } = string.Empty;
        public ResetPasswordCommand(Email email, string token, string newPassword, int? requestedBy = null)
        {
            Email = email;
            Token = token;
            NewPassword = newPassword;
            RequestedBy = requestedBy;
        }

    }
}
