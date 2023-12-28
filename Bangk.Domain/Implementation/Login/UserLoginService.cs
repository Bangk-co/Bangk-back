using FluentValidation;
using Bangk.Common.Dto;
using Bangk.Common.Utils;
using Bangk.Domain.Contract.Login;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Login
{
    public class UserLoginService : DomainBase<User>, IUserLoginService
    {
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly ICreateTokenService _createTokenService;
        public UserLoginService(IUserRepository repository, IValidator<LoginDto> loginValidator, ICreateTokenService createTokenService) : base(repository)
        {
            _loginValidator = loginValidator;
            _createTokenService = createTokenService;
        }

        public async Task<ResponseDto<string>> Execute(LoginDto login)
        {
            var result = new ResponseDto<string>();
            var validationResult = await _loginValidator.ValidateAsync(login);

            if (!validationResult.IsValid)
            {
                result.HasErrors = true;

                foreach (var error in validationResult.Errors)
                    result.Errors.Add(error.ErrorMessage);

                return result;
            }

            var user = await Repository.FirstOfDefaultAsync(x => x.Email == login.Email, x => x.Role);
            if (user == null || !PasswordUtils.VerifyPasswordHash(login.Password, user.PasswordHash, user.PasswordSalt))
            {
                result.HasErrors = true;
                result.Errors.Add("Invalid email or password");
                return result;
            }

            result.Result = _createTokenService.Execute(user);

            return result;
        }
    }
}
