using Bangk.Common.Dto;
using FluentValidation;

namespace Bangk.Common.Validators
{
    public class CreateUserValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("El contraseña es obligatorio");
            RuleFor(x => x.PasswordConfirm)
               .NotEmpty().WithMessage("La confirmacion de la contraseña obligatorio");
            RuleFor(x => x.ClientId)
             .NotEmpty().WithMessage("El cliente es obligatorio");
        }
    }
}
