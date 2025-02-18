using Bangk.Common.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Validators
{
    public class CreateMedPersonInsuredValidator : AbstractValidator<MedPersonInsuredsDto>
    {

        public CreateMedPersonInsuredValidator()
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
            RuleFor(x => x.MedEnsuraceApplicationId)
              .NotNull().WithMessage("La información de la solitud es obligatorio");

            RuleFor(dto => dto.MedEnsuranceApplicationDto).SetValidator(new CreateMedEnsuraceApplicationValidator());
        }

    }
    public class CreateMedEnsuraceApplicationValidator : AbstractValidator<CreateMedEnsuranceApplicationDto>
    {
        public CreateMedEnsuraceApplicationValidator()
        {
            RuleFor(dto => dto.MethodPaymentId).GreaterThan(0).WithMessage("El metodo de pago es obligatorio"); ;
        }
    }
}
