using FluentValidation;
using Bangk.Common.Dto;
using Bangk.Common.Utils;
using Serilog;
using System;

namespace Bangk.Common.Validators
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Hash)
               .NotEmpty().WithMessage("El has para el cambio de contraseña es obligatorio");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria");
            RuleFor(x => x.PasswordConfirm)
              .NotEmpty().WithMessage("La confirmacion de la contraseña es obligatorio");
        }
    }
}
