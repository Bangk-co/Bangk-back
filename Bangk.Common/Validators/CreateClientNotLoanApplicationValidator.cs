using Bangk.Common.Dto;
using FluentValidation;

namespace Bangk.Common.Validators
{
    public class CreateClientNotLoanApplicationValidator : AbstractValidator<CreateClientNotLoanApplicationDto>
    {
        public CreateClientNotLoanApplicationValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo del cliente es obligatorio");
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Ingrese un correo válido");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del cliente es obligatorio");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("El apellido del cliente es obligatorio");
            RuleFor(x => x.IdentificationNumber)
                .NotEmpty().WithMessage("El número de identificación del cliente es obligatorio");
            RuleFor(x => x.DocumenIdentificationTypeId)
                .GreaterThan(0).WithMessage("El tipo de identificación  del cliente es obligatorio");
        }
    }
}
