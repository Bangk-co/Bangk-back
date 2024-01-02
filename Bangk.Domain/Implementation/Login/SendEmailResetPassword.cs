using Bangk.Business.Extension.SenderGrid;
using Bangk.Common.Dto;
using Bangk.Domain.Contract.Login;
using Bangk.Domain.Contract.Users;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Login
{
    public class SendEmailResetPassword : ISendEmailResetPassword
    {
        private readonly IConfiguration _configuration;
        private readonly IGetUserByEmailService _getUserByEmailService;
        private readonly IUserLoginService _userLoginService;
        public SendEmailResetPassword(IConfiguration configuration, IGetUserByEmailService getUserByEmailService, IUserLoginService userLoginService)
        {
            _configuration = configuration;
            _getUserByEmailService = getUserByEmailService;
            _userLoginService = userLoginService;
        }
        public async Task<ResponseDto<bool>> Execute(string email)
        {
            var result = new ResponseDto<bool>();
            var user = _getUserByEmailService.Execute(email);
            if (user == null)
            {
                result.HasErrors = true;
                result.Errors.Add($"The email {email} does not exist.");
                return result;
            }

            user.PasswordResetGuid = Guid.NewGuid().ToString();
            await _userLoginService.Update(user);
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
