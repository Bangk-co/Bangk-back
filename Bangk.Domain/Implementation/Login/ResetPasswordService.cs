using Bangk.Common;
using Bangk.Common.Dto;
using Bangk.Common.Utils;
using Bangk.Domain.Contract.Login;
using Bangk.Domain.Contract.Users;
using Bangk.Repositories.Repositories;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Login
{
    public class ResetPasswordService : IResetPasswordService
    {
        private readonly IValidator<ResetPasswordDto> _resetPasswordValidator;
        private readonly IUserRepository _userRepository;
        private readonly IGetUserByHashEmailResetService _getUserByHashEmailResetService;
        private readonly IConfiguration _configuration;
        public ResetPasswordService(IValidator<ResetPasswordDto> resetPasswordValidator,
             IUserRepository userRepository, IGetUserByEmailService getUserByEmailService, IGetUserByHashEmailResetService getUserByHashEmailResetService,
            IConfiguration configuration)
        {
            _resetPasswordValidator = resetPasswordValidator;
            _userRepository = userRepository;
            _getUserByHashEmailResetService = getUserByHashEmailResetService;
            _configuration = configuration;
        }

        public async Task<ResponseDto<bool>> Execute(ResetPasswordDto resetPasswordDto)
        {
            var result = new ResponseDto<bool>();
            var validationResult = await _resetPasswordValidator.ValidateAsync(resetPasswordDto);
            if (!validationResult.IsValid)
            {
                result.HasErrors = true;

                foreach (var error in validationResult.Errors)
                    result.Errors.Add(error.ErrorMessage);
                return result;
            }

            var user = _getUserByHashEmailResetService.Execute(resetPasswordDto.Hash);
            if (user == null)
            {
                result.HasErrors = true;
                result.Errors.Add($"El link para el cambio de contraña ha expirado.");
                return result;
            }
            try
            {
                var key = _configuration.GetSection("PasswordConfig:Key").Value;
                var iv = _configuration.GetSection("PasswordConfig:IV").Value;
                var password = PasswordUtils.DecryptPassword(resetPasswordDto.Password, key, iv);
                var passwordConfirm = PasswordUtils.DecryptPassword(resetPasswordDto.PasswordConfirm, key, iv);
                if (!password.Equals(passwordConfirm))
                {
                    result.HasErrors = true;
                    result.Errors.Add($"El password y la confirmacion del password no coinciden.");
                    return result;
                }

                PasswordUtils.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
                user.PasswordSalt = passwordSalt;
                user.PasswordHash = passwordHash;
                user.PasswordResetGuid = null;
                await _userRepository.Update(user);
                result.Result = true;
                return result;
            }
            catch (Exception ex)
            {
                Logs.WriteErrorLog($"No fue posible desencriptar la contraseña: [{ex.Message}].");
                result.HasErrors = true;
                result.Errors.Add("No fue posible desencriptar la contraseña");
                return result;
            }
        }
    }
}
