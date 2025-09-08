using ELearningIskoop.Users.Application.Commands.LoginUser;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
    {
        public UpdateUserProfileCommandValidator()
        {

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("İsim zorunludur")
                .MaximumLength(50).WithMessage("İsim en fazla 50 karakter olabilir")
                .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$").WithMessage("İsim sadece harf ve boşluk içerebilir");


            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyad zorunludur")
                .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir")
                .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$").WithMessage("Soyad sadece harf ve boşluk içerebilir");

            RuleFor(x => x.Bio)
                .MaximumLength(500).WithMessage("Biyografi en fazla 500 karakter olabilir")
                .Matches(@"^[a-zA-Z0-9ğüşıöçĞÜŞİÖÇ\s.,!?-]+$").WithMessage("Biyografi geçersiz karakterler içeriyor");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Telefon numarası zorunludur")
                .Matches(@"^(\+90|0)?[5][0-9]{9}$").WithMessage("Geçerli bir Türkiye telefon numarası giriniz")
                .MaximumLength(13).WithMessage("Telefon numarası en fazla 13 karakter olabilir");

            RuleFor(x => x.ProfilePictureUrl)
                .MaximumLength(500).WithMessage("Profil resmi URL'si çok uzun")

                .Must(BeAValidUrl).WithMessage("Geçerli bir URL giriniz")

                .When(x => !string.IsNullOrEmpty(x.ProfilePictureUrl)); // Sadece doluysa kontrol et
        }
        private bool BeAValidUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return true;

            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

    }
}
