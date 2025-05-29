using Bangk.Business.Extension.SenderGrid;
using Bangk.Common.Dto;
using Bangk.Common.Utils;
using Bangk.Domain.Contract.Login;
using Bangk.Repositories.Repositories;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Login
{
    public class SendCodeValidationEmailService : ISendCodeValidationEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IValidator<SendCodeValidationEmailDto> _sendCodeValidationEmailValidator;

        public SendCodeValidationEmailService(IConfiguration configuration, IUserRepository userRepository, IClientRepository clientRepository,
            IValidator<SendCodeValidationEmailDto> sendCodeValidationEmailValidator)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _sendCodeValidationEmailValidator = sendCodeValidationEmailValidator;
        }

        public async Task<ResponseDto<string>> Execute(SendCodeValidationEmailDto sendCodeValidationEmailDto)
        {
            var result = new ResponseDto<string>();
            var validationResult = await _sendCodeValidationEmailValidator.ValidateAsync(sendCodeValidationEmailDto);
            if (!validationResult.IsValid)
            {
                result.HasErrors = true;

                foreach (var error in validationResult.Errors)
                    result.Errors.Add(error.ErrorMessage);

                return result;
            }

            var client = await _clientRepository.ExistClientByEmailAndIdentificationNumber(sendCodeValidationEmailDto.Email, sendCodeValidationEmailDto.IdentificationNumber);
            if (client)
            {
                result.HasErrors = true;
                result.Errors.Add($"El cliente ingresado ya existe");
                return result;
            }

            var key = _configuration.GetSection("PasswordConfig:Key").Value;
            var iv = _configuration.GetSection("PasswordConfig:IV").Value;
            var code = PasswordUtils.GenerateRandomNumber().ToString();
            var codeEncrypted = PasswordUtils.EncryptAes(code, key, iv);
            var sendEmail = new SendGridEmailDto
            {
                Email = sendCodeValidationEmailDto.Email,
                From = _configuration.GetSection("SendGridEmailSender:From").Value,
                Key = _configuration.GetSection("SendGridEmailSender:Key").Value,
                Name = _configuration.GetSection("SendGridEmailSender:Name").Value,
                TemplateId = _configuration.GetSection("SendGridEmailSender:ValidationCodeEmail").Value,
                Data = new
                {
                    clientname = $"{sendCodeValidationEmailDto.Name}",
                    code = $"{code}"
                }
            };
               
            await SendGridEmailSender.SendEmailAsync(sendEmail);
            result.Result = codeEncrypted;
            return result;
        }
    }
}
