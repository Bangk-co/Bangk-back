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
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(sendGridEmailDto.From, sendGridEmailDto.Name),
                Subject = sendGridEmailDto.subject,
                PlainTextContent = sendGridEmailDto.htmlMessage,
                HtmlContent = sendGridEmailDto.htmlMessage
            };

            msg.AddTo(new EmailAddress(sendGridEmailDto.email));
            var response = await client.SendEmailAsync(msg);
            Logs.WriteInfoLog(response.IsSuccessStatusCode
                               ? $"Email to {sendGridEmailDto.email} queued successfully!"
                               : $"Failure Email to {sendGridEmailDto.email}");
        }
    }
}
