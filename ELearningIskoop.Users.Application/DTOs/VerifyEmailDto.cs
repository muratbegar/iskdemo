using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.DTOs
{
    public record VerifyEmailDto
    {
        public int UserId { get; init; }
        public string VerificationToken { get; init; } = string.Empty;
    }
}
