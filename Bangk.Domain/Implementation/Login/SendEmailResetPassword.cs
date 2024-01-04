using Bangk.Business.Extension.SenderGrid;
using Bangk.Common.Dto;
using Bangk.Domain.Contract.Login;
using Bangk.Domain.Contract.Users;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Login
{
    public class SendEmailResetPassword : ISendEmailResetPassword
    {
        private readonly IConfiguration _configuration;
        private readonly IGetUserByEmailService _getUserByEmailService;
        private readonly IUserRepository _userRepository;
        public SendEmailResetPassword(IConfiguration configuration, IGetUserByEmailService getUserByEmailService, IUserRepository userRepository)
        {
            _configuration = configuration;
            _getUserByEmailService = getUserByEmailService;
            _userRepository = userRepository;
        }
        public async Task<ResponseDto<bool>> Execute(string email)
        {
            var result = new ResponseDto<bool>();
            var user = _getUserByEmailService.Execute(email);
            if (user == null)
            {
                result.HasErrors = true;
                result.Errors.Add($"El correo ingresado no existe");
                return result;
            }

            user.PasswordResetGuid = Guid.NewGuid().ToString();
            await _userRepository.Update(user);
            var url = _configuration.GetSection("ResetPassword:UrlFront").Value + $"?hash={user.PasswordResetGuid}&email={user.Email}";

            var sendEmail = new SendGridEmailDto
            {
                email = email,
                From = _configuration.GetSection("SendGridEmailSender:From").Value,
                Key = _configuration.GetSection("SendGridEmailSender:Key").Value,
                Name = _configuration.GetSection("SendGridEmailSender:Name").Value,
                TemplateId = _configuration.GetSection("SendGridEmailSender:TemplateResetPassword").Value,
                Data = new
                {
                    username = $"{user.Name}",
                    UrlCallback = url
                }
            };
               
            await SendGridEmailSender.SendEmailAsync(sendEmail);
            result.Result = true;
            return result;
        }
    }
}
