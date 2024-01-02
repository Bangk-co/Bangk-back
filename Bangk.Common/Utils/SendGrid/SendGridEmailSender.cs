using Bangk.Common;
using Bangk.Common.Dto;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace Bangk.Business.Extension.SenderGrid
{
    public static class SendGridEmailSender
    {
        public static async Task SendEmailAsync(SendGridEmailDto sendGridEmailDto)
        {
            string sendGridApiKey = sendGridEmailDto.Key;
            if (string.IsNullOrEmpty(sendGridApiKey))
            {
                throw new Exception("The 'SendGridApiKey' is not configured");
            }

            var client = new SendGridClient(sendGridApiKey);
            var msg = MailHelper.CreateSingleTemplateEmail(new EmailAddress(sendGridEmailDto.From, sendGridEmailDto.Name), new EmailAddress(sendGridEmailDto.email),
                sendGridEmailDto.TemplateId, sendGridEmailDto.Data);

            var response = await client.SendEmailAsync(msg);
            Logs.WriteInfoLog(response.IsSuccessStatusCode
                               ? $"Email to {sendGridEmailDto.email} queued successfully!"
                               : $"Failure Email to {sendGridEmailDto.email}");
        }
    }
}
