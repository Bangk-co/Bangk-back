using Bangk.Common.Dto;
using FluentValidation;

namespace Bangk.Common.Validators
{
    public class CreateClientValidator : AbstractValidator<CreateClientDto>
    {
        public CreateClientValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo del cliente es obligatorio");
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Ingrese un correo válido");
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("El número de celular del cliente es obligatorio");
            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("El fecha de nacimiento del cliente es obligatorio");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del cliente es obligatorio");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("El apellido del cliente es obligatorio");
            RuleFor(x => x.IdentificationNumber)
                .NotEmpty().WithMessage("El número de identificación del cliente es obligatorio");
            RuleFor(x => x.GenderId)
                .GreaterThan(0).WithMessage("El género del cliente es obligatorio");
            RuleFor(x => x.CountryCodePhoneId)
                .GreaterThan(0).WithMessage("El codigo del país es obligatorio");
            RuleFor(x => x.DocumenIdentificationTypeId)
                .GreaterThan(0).WithMessage("El tipo de identificación  del cliente es obligatorio");
            RuleFor(x => x.IdentificationFile)
             .NotNull().WithMessage("El archivo del la cedula del cliente es obligatorio");

            RuleFor(x => x.LoadAplicacionDto)
              .NotNull().WithMessage("La información de la solitud es obligatorio");

            RuleFor(dto => dto.LoadAplicacionDto).SetValidator(new CreateLoadAplicacionDtoValidator());
        }
    }

    public class CreateLoadAplicacionDtoValidator : AbstractValidator<CreateLoadAplicacionDto>
    {
        public CreateLoadAplicacionDtoValidator()
        {
            RuleFor(dto => dto.BankStatementFile).NotNull().WithMessage("El extracto del crédito es obligatorio"); ;
            RuleFor(dto => dto.BankId).GreaterThan(0).WithMessage("El banco al que pertenece el crédito es obligatorio"); ;
            RuleFor(dto => dto.LoanTypeId).GreaterThan(0).WithMessage("El tipo de crédito es obligatorio"); ;
            RuleFor(dto => dto.LoanBalance).GreaterThan(0).WithMessage("El saldo del crédito es obligatorio"); ;
            RuleFor(dto => dto.LoanNumber).NotEmpty().WithMessage("El número del crédito es obligatorio"); ;
        }
    }
}
