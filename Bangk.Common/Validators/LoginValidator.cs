using FluentValidation;
using Bangk.Common.Dto;

namespace Bangk.Common.Validators
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo es obligatorio");
            RuleFor(x => x.Email)
                 .EmailAddress().WithMessage("Ingrese un correo valido");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria");
          
        }
    }
}
