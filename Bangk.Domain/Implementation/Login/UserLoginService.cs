using Bangk.Common.Dto;
using Bangk.Common.Utils;
using Bangk.Domain.Contract.Login;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Login
{
    public class UserLoginService :  IUserLoginService
    {
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly ICreateTokenService _createTokenService;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _repository;
        public UserLoginService(IUserRepository repository, IValidator<LoginDto> loginValidator, ICreateTokenService createTokenService, IConfiguration configuration)
        {
            _loginValidator = loginValidator;
            _createTokenService = createTokenService;
            _configuration = configuration;
            _repository = repository;
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

            var user = await _repository.FirstOfDefaultAsync(x => x.Email == login.Email, x => x.Role);
            var key = _configuration.GetSection("PasswordConfig:Key").Value;
            var iv = _configuration.GetSection("PasswordConfig:IV").Value;
            try
            {
                var password = PasswordUtils.DecryptPassword(login.Password, key, iv);
                if (user == null || !PasswordUtils.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                {
                    result.HasErrors = true;
                    result.Errors.Add("correo o contraseña inválidos");
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.HasErrors = true;
                result.Errors.Add("correo o contraseña inválidos");
                return result;
            }

            result.Result = _createTokenService.Execute(user);

            return result;
        }
    }
}
