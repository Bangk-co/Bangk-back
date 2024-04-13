using FluentValidation;
using Bangk.Common.Dto;

namespace Bangk.Common.Validators
{
    public class SendCodeValidationEmailValidator : AbstractValidator<SendCodeValidationEmailDto>
    {
        public SendCodeValidationEmailValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo es obligatorio");
            RuleFor(x => x.Email)
                 .EmailAddress().WithMessage("Ingrese un correo válido");
            RuleFor(x => x.IdentificationNumber)
                .NotEmpty().WithMessage("El número  de identificación es obligatorio");
          
        }
    }
}
