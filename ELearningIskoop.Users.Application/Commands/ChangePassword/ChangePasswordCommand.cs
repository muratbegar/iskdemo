using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.Users.Application.DTOs;

namespace ELearningIskoop.Users.Application.Commands.ChangePassword
{
    public record ChangePasswordCommand : BaseCommand<ChangePasswordDto>, ICommand
    {
        public int UserId { get; init; }
        public string CurrentPassword { get; init; } = string.Empty;
        public string NewPassword { get; init; } = string.Empty;
        public string ConfirmNewPassword { get; init; } = string.Empty;
    }
}
