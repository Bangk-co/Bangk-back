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
            var msg = MailHelper.CreateSingleTemplateEmail(new EmailAddress(sendGridEmailDto.From, sendGridEmailDto.Name), new EmailAddress(sendGridEmailDto.Email.Trim()),
                sendGridEmailDto.TemplateId, sendGridEmailDto.Data);

            if (!string.IsNullOrEmpty(sendGridEmailDto.Cc))
            {
                msg.AddCc(sendGridEmailDto.Cc);
            }
            
            if (sendGridEmailDto.Attachments != null && sendGridEmailDto.Attachments.Count > 0)
            {
                foreach (var attachment in sendGridEmailDto.Attachments)
                {
                    msg.AddAttachment(attachment.Filename, attachment.Content, attachment.Type);
                }
            }

            var response = await client.SendEmailAsync(msg);
        }
    }
}
