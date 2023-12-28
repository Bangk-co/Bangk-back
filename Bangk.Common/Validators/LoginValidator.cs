using FluentValidation;
using Bangk.Common.Dto;

namespace Bangk.Common.Validators
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("The email is required");
            RuleFor(x => x.Email)
                 .EmailAddress().WithMessage("Enter a valid email ");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("The password is required");
          
        }
    }
}
