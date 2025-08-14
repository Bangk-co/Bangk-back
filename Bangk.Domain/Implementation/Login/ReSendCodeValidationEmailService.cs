using Bangk.Business.Extension.SenderGrid;
using Bangk.Common.Dto;
using Bangk.Common.Utils;
using Bangk.Domain.Contract.Login;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Login
{
    public class ReSendCodeValidationEmailService : IReSendCodeValidationEmailService
    {
        private readonly IConfiguration _configuration;
        public ReSendCodeValidationEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ResponseDto<string>> Execute(string email)
        {
            var result = new ResponseDto<string>();
            if (string.IsNullOrEmpty(email))
            {
                result.HasErrors = true;
                result.Errors.Add($"El correo es obligatorio");
                return result;
            }

            if (!PasswordUtils.IsValidEmail(email))
            {
                result.HasErrors = true;
                result.Errors.Add($"Ingrese un correo válido");
                return result;
            }

            var key = _configuration.GetSection("PasswordConfig:Key").Value;
            var iv = _configuration.GetSection("PasswordConfig:IV").Value;
            var code = PasswordUtils.GenerateRandomNumber().ToString();
            var codeEncripted = PasswordUtils.EncryptAes(code, key, iv);
            var sendEmail = new SendGridEmailDto
            {
                Email = email,
                From = _configuration.GetSection("SendGridEmailSender:From").Value,
                Key = _configuration.GetSection("SendGridEmailSender:Key").Value,
                Name = _configuration.GetSection("SendGridEmailSender:Name").Value,
                TemplateId = _configuration.GetSection("SendGridEmailSender:ValidationCodeEmail").Value,
                Data = new
                {
                    code = $"{code}"
                }
            };
               
            await SendGridEmailSender.SendEmailAsync(sendEmail);
            result.Result = codeEncripted;
            return result;
        }
    }
}
