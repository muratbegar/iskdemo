using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.Users.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Commands.ForgotPassword
{
    public record ForgotPasswordCommand : BaseCommand<ForgotPasswordDto>, ICommand
    {
        public string Email { get; init; } = string.Empty;
    }

    public record ForgotPasswordDto
    {
      
        public string Email { get; init; } = string.Empty;

    }
}
