using FluentValidation;
using Bangk.Common.Dto;
using Bangk.Common.Utils;
using Bangk.Domain.Contract.Login;
using Bangk.Domain.Contract.Users;
using Bangk.Repositories.Repositories;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Login
{
    public class ResetPasswordService : IResetPasswordService
    {
        private readonly IValidator<ResetPasswordDto> _resetPasswordValidator;
        private readonly IUserRepository _userRepository;
        private readonly IGetUserByEmailService _getUserByEmailService;
        private readonly IGetUserByHashEmailResetService _getUserByHashEmailResetService;
        public ResetPasswordService(IValidator<ResetPasswordDto> resetPasswordValidator,
             IUserRepository userRepository, IGetUserByEmailService getUserByEmailService, IGetUserByHashEmailResetService getUserByHashEmailResetService)
        {
            _resetPasswordValidator = resetPasswordValidator;
            _userRepository = userRepository;
            _getUserByEmailService = getUserByEmailService;
            _getUserByHashEmailResetService = getUserByHashEmailResetService;
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
                result.Errors.Add($"The reset password link has been expired.");
                return result;
            }

            PasswordUtils.CreatePasswordHash(resetPasswordDto.Password, out var passwordHash, out var passwordSalt);
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;
            user.PasswordResetGuid = null;
            await _userRepository.Update(user);
            result.Result = true;
            return result;
        }
    }
}
