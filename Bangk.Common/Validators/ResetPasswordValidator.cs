using FluentValidation;
using Bangk.Common.Dto;

namespace Bangk.Common.Validators
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Hash)
               .NotEmpty().WithMessage("The hash value is required");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("The password is required");
            RuleFor(x => x.PasswordConfirm)
              .NotEmpty().WithMessage("The password confirm is required");
            RuleFor(x => x.PasswordConfirm)
              .Equal(x => x.Password).WithMessage("Passwords do not match");
        }
    }
}
