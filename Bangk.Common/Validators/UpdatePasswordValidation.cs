using Bangk.Common.Dto;
using FluentValidation;

namespace Bangk.Common.Validators
{
    public class UpdatePasswordValidation : AbstractValidator<UpdatePasswordDto>
    {
        public UpdatePasswordValidation()
        {
            RuleFor(x => x.Password).NotEmpty().WithMessage("la contraseña es obligatorio.");
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("El campo confirmar contraseña es obligatorios.");
            RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("la contraseña actual es obligatorio.");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("El usuario es obligatorio.");
        }
    }
}
