using Bangk.Common.Dto;
using FluentValidation;

namespace Bangk.Common.Validators
{
    public class UpdateLoanApplicationValidator : AbstractValidator<UpdateLoanApplicationDto>
    {
        public UpdateLoanApplicationValidator()
        {
            RuleFor(dto => dto.BankId).GreaterThan(0).WithMessage("El banco al que pertenece el crédito es obligatorio"); 
            RuleFor(dto => dto.LoanTypeId).GreaterThan(0).WithMessage("El tipo de crédito es obligatorio"); 
            RuleFor(dto => dto.LoanBalance).GreaterThan(0).WithMessage("El saldo del crédito es obligatorio"); 
            RuleFor(dto => dto.LoanNumber).GreaterThan(0).WithMessage("El número del crédito es obligatorio");
            RuleFor(dto => dto.MonthlyCostInsurance).GreaterThan(0).WithMessage("El costo mensual del seguro es obligatorio"); 
        }
    }

}
