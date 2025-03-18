using Bangk.Common.Dto;
using FluentValidation;

namespace Bangk.Common.Validators
{
    public class CreatePetInsuranceApplicationValidator : AbstractValidator<PetInsuranceDto>
    {

        public CreatePetInsuranceApplicationValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo del cliente es obligatorio");
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Ingrese un correo válido");
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("El número de celular del cliente es obligatorio");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del cliente es obligatorio");
            RuleFor(x => x.DocumentNumber)
                .NotEmpty().WithMessage("El número de identificación del cliente es obligatorio");
            RuleFor(x => x.DocumentTypeId)
                .GreaterThan(0).WithMessage("El tipo de identificación  del cliente es obligatorio");
            RuleFor(x => x.MethodPaymentId)
              .GreaterThan(0).WithMessage("El tipo de identificación  del cliente es obligatorio");
        }
    }
}
