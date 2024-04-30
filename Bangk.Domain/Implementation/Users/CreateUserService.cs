using Bangk.Common.Dto;
using Bangk.Common.Utils;
using Bangk.Domain.Contract.Users;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Users
{
    public class CreateUserService : ICreateUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IValidator<CreateUserDto> _createUserDtoValidator;
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;
        public CreateUserService(IConfiguration configuration, IValidator<CreateUserDto> createUserDtoValidator, IUserRepository userRepository, IClientRepository clientRepository)
        {
            _configuration = configuration;
            _createUserDtoValidator = createUserDtoValidator;
            _userRepository = userRepository;
            _clientRepository = clientRepository;
        }

        public async Task<ResponseDto<bool>> Execute(CreateUserDto createUserDto)
        {
            string key = _configuration.GetSection("PasswordConfig:Key").Value;
            string iv = _configuration.GetSection("PasswordConfig:IV").Value;
            var result = new ResponseDto<bool>();

            var validationResult = await _createUserDtoValidator.ValidateAsync(createUserDto);
            if (!validationResult.IsValid)
            {
                result.HasErrors = true;

                foreach (var error in validationResult.Errors)
                    result.Errors.Add(error.ErrorMessage);
                return result;
            }

            var password = PasswordUtils.DecryptPassword(createUserDto.Password, key, iv);
            var passwordConfirm = PasswordUtils.DecryptPassword(createUserDto.PasswordConfirm, key, iv);

            if (!string.Equals(password, passwordConfirm))
            {
                result.HasErrors = true;
                result.Errors.Add($"El password y la confirmacion del password no coinciden.");
                return result;
            }

            var client = await _clientRepository.GetById(createUserDto.ClientId);
            if (client == null)
            {
                result.HasErrors = true;
                result.Errors.Add($"El cliente no existe.");
                return result;
            }

            PasswordUtils.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
            var user = new User
            {
                PasswordSalt = passwordSalt,
                PasswordHash = passwordHash,
                Email = client.Email.Trim(),
                LastName = client.LastName,
                Name = client.Name,
                RoleId = 3,
                ClientId = client.Id,
            };
            
            await _userRepository.Create(user);
            result.Result = true;
            return result;
        }
    }
}
