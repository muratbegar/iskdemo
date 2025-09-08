using ELearningIskoop.Users.Application.Commands.ChangePassword;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Commands.VerifyEmail
{
    public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
    {
        public VerifyEmailCommandValidator()
        {
            RuleFor(x => x.VerificationToken)
                .NotEmpty().WithMessage("Doğrulama token'ı zorunludur")
                .Length(6).WithMessage("Doğrulama token'ı 6 karakter olmalıdır")
                .Matches(@"^[0-9]+$").WithMessage("Doğrulama token'ı sadece rakamlardan oluşmalıdır");
        }
    }
}
