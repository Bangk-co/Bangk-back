using Bangk.Common.Dto;
using Bangk.Common.Utils;
using Bangk.Domain.Contract.Users;
using Bangk.Repositories.Repositories;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Users
{
    public class UpdatePasswordService : IUpdatePasswordService
    {
        private readonly IValidator<UpdatePasswordDto> _updateUserDtoValidator;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        public UpdatePasswordService(IConfiguration configuration, IUserRepository userRepository, IValidator<UpdatePasswordDto> updateUserDtoValidator)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _updateUserDtoValidator = updateUserDtoValidator;
        }

        public async Task<ResponseDto<bool>> Execute(UpdatePasswordDto updatePasswordDto)
        {
            string key = _configuration.GetSection("PasswordConfig:Key").Value;
            string iv = _configuration.GetSection("PasswordConfig:IV").Value;
            var result = new ResponseDto<bool>();

            var validationResult = await _updateUserDtoValidator.ValidateAsync(updatePasswordDto);
            if (!validationResult.IsValid)
            {
                result.HasErrors = true;

                foreach (var error in validationResult.Errors)
                    result.Errors.Add(error.ErrorMessage);
                return result;
            }

            var user = await _userRepository.GetById(updatePasswordDto.UserId);
            if (user == null)
            {
                result.HasErrors = true;
                result.Errors.Add($"El usuario no existe en la aplicación.");
                return result;
            }

            var currentPassword = PasswordUtils.DecryptPassword(updatePasswordDto.CurrentPassword, key, iv);
            var isCorrectLogin = PasswordUtils.IsValidPassword(currentPassword);
            if (!isCorrectLogin)
            {
                result.HasErrors = true;
                result.Errors.Add($"La contraseña actual no es valida.");
                return result;
            }

            var password = PasswordUtils.DecryptPassword(updatePasswordDto.Password, key, iv);
            var passwordConfirm = PasswordUtils.DecryptPassword(updatePasswordDto.ConfirmPassword, key, iv);
            if (!string.Equals(password, passwordConfirm))
            {
                result.HasErrors = true;
                result.Errors.Add($"El password y la confirmacion del password no coinciden.");
                return result;
            }

            if (string.Equals(password, currentPassword))
            {
                result.HasErrors = true;
                result.Errors.Add($"Las contraseña nueva no puede ser igual a la anterior.");
                return result;
            }

            PasswordUtils.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _userRepository.Update(user);
            result.Result = true;
            return result;
        }
    }
}
