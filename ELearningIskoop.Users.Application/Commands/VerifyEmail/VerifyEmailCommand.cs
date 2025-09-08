using ELearningIskoop.BuildingBlocks.Application.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Application.DTOs;

namespace ELearningIskoop.Users.Application.Commands.VerifyEmail
{
    public record VerifyEmailCommand : BaseCommand<VerifyEmailDto>,ICommand
    {
        public int UserId { get; init; }
        public string VerificationToken { get; init; } = string.Empty;
    }
}
