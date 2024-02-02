using Bangk.Common.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Validators
{

    public class UpdateClientValidator : AbstractValidator<ClientUpdateInfoDto>
    {
        public UpdateClientValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo del cliente es obligatorio");
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Ingrese un correo válido");
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("El número de celular del cliente es obligatorio");
            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("La fecha de nacimiento del cliente es obligatorio");
            RuleFor(x => x.DocumentIssueDate)
                .NotEmpty().WithMessage("La fecha de expedición del cliente es obligatorio");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del cliente es obligatorio");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("El apellido del cliente es obligatorio");
            RuleFor(x => x.IdentificationNumber)
                .NotEmpty().WithMessage("El número de identificación del cliente es obligatorio");
            RuleFor(x => x.GenderId)
                .GreaterThan(0).WithMessage("El género del cliente es obligatorio");
            RuleFor(x => x.CountryCode)
                .GreaterThan(0).WithMessage("El codigo del país es obligatorio");
            RuleFor(x => x.DocumentTypeId)
                .GreaterThan(0).WithMessage("El tipo de identificación  del cliente es obligatorio");
            
        }
    }
}
