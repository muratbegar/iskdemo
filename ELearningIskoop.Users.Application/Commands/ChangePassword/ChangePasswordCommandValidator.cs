using ELearningIskoop.Users.Application.Commands.RegisterUser;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Commands.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            // CurrentPassword
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Mevcut şifre zorunludur")
                .MinimumLength(6).WithMessage("Mevcut şifre en az 6 karakter olmalıdır")
                .MaximumLength(100).WithMessage("Mevcut şifre en fazla 100 karakter olabilir");

            // NewPassword
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Yeni şifre zorunludur")
                .MinimumLength(8).WithMessage("Yeni şifre en az 8 karakter olmalıdır")
                .MaximumLength(100).WithMessage("Yeni şifre en fazla 100 karakter olabilir")
                .Matches(@"[A-Z]").WithMessage("Yeni şifre en az 1 büyük harf içermelidir")
                .Matches(@"[a-z]").WithMessage("Yeni şifre en az 1 küçük harf içermelidir")
                .Matches(@"\d").WithMessage("Yeni şifre en az 1 rakam içermelidir")
                .Matches(@"[!@#$%^&*()[\]{};:'"",.<>/?\\|`~_\-+=]").WithMessage("Yeni şifre en az 1 özel karakter içermelidir")
                .NotEqual(x => x.CurrentPassword).WithMessage("Yeni şifre mevcut şifreden farklı olmalıdır")
                .Must(password => CheckPasswordStrength(password) >= PasswordStrength.Medium)
                    .WithMessage("Yeni şifre en az 'Orta' güç seviyesinde olmalıdır");

            // ConfirmNewPassword
            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("Yeni şifre tekrarı zorunludur")
                .Equal(x => x.NewPassword).WithMessage("Yeni şifre ve tekrarı eşleşmiyor");
        }

        private PasswordStrength CheckPasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password))
                return PasswordStrength.VeryWeak;

            int score = 0;

            // Length
            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;
            if (password.Length >= 16) score++;

            // Complexity
            if (Regex.IsMatch(password, @"[a-z]")) score++;
            if (Regex.IsMatch(password, @"[A-Z]")) score++;
            if (Regex.IsMatch(password, @"\d")) score++;
            if (Regex.IsMatch(password, @"[!@#$%^&*()[\]{};:'"",.<>/?\\|`~_\-+=]")) score++;

            // Common patterns (negative score)
            if (Regex.IsMatch(password, @"(.)\1{2,}")) score--; // tekrar eden karakterler
            if (Regex.IsMatch(password, @"(012|123|234|345|456|567|678|789|890|abc|bcd|cde|def)")) score--;

            // Çok kullanılan şifreler
            var commonPasswords = new[] { "123456", "password", "qwerty", "admin", "letmein" };
            if (commonPasswords.Contains(password.ToLower()))
                return PasswordStrength.VeryWeak;

            // Negatif skorları sıfıra çek
            score = Math.Max(score, 0);

            return score switch
            {
                >= 7 => PasswordStrength.VeryStrong,
                >= 5 => PasswordStrength.Strong,
                >= 3 => PasswordStrength.Medium,
                >= 1 => PasswordStrength.Weak,
                _ => PasswordStrength.VeryWeak
            };
        }
    }
    public enum PasswordStrength
    {
        VeryWeak = 0,
        Weak = 1,
        Medium = 2,
        Strong = 3,
        VeryStrong = 4
    }
}
