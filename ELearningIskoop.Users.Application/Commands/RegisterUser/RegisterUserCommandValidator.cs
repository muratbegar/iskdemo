using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace ELearningIskoop.Users.Application.Commands.RegisterUser
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email adresi zorunludur").EmailAddress()
                .WithMessage("Geçerli bir mail adresi giriniz.").MaximumLength(254)
                .WithMessage("Email adresi çok uzun");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("İsim zorunludur")
                .MinimumLength(2).WithMessage("İsim en az 2 karakter olmalıdır")
                .MaximumLength(50).WithMessage("İsim en fazla 50 karakter olabilir")
                .Matches(@"^[a-zA-ZğüşöçıİĞÜŞÖÇ\s]+$").WithMessage("İsim sadece harf içermelidir");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyad zorunludur")
                .MinimumLength(2).WithMessage("Soyad en az 2 karakter olmalıdır")
                .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir")
                .Matches(@"^[a-zA-ZğüşöçıİĞÜŞÖÇ\s]+$").WithMessage("Soyad sadece harf içermelidir");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre zorunludur")
                .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır")
                .Matches(@"[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir")
                .Matches(@"[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir")
                .Matches(@"[0-9]").WithMessage("Şifre en az bir rakam içermelidir")
                .Matches(@"[\!\?\*\.\@\#\$\%\^\&\+\=]").WithMessage("Şifre en az bir özel karakter içermelidir");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Şifreler eşleşmiyor");

        }
    }
}
