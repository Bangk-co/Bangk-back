using Bangk.Business.Extension.SenderGrid;
using Bangk.Common;
using Bangk.Common.Dto;
using Bangk.Domain.Contract;
using Bangk.Domain.Contract.Sftp;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Bangk.Domain.Implementation.BackgroundTask
{
    public class SFTPConnection : ISFTPConnection
    {
        private readonly SftpClient _client;
        private readonly IConfiguration _configuration;
        private readonly ISettingRepository _settingRepository;
        private readonly IReportExcel _reportExcel;
         

        public SFTPConnection(SftpClient client, IConfiguration configuration, ISettingRepository settingRepository, IReportExcel reportExcel)

        {
            _client = client;
            _configuration = configuration;
            _settingRepository = settingRepository;
            _reportExcel = reportExcel;

        }

        public Task ConnectAsync()
        {
            try
            {
                _client.Connect();
                if (!_client.IsConnected)
                {
                    throw new SftpConnectionException("No se pudo establecer la conexión al servidor SFTP.");
                }
            }
            catch (SshAuthenticationException ex)
            {
                
                Logs.WriteErrorLog($"Error de autenticación: {ex.Message}");
                
            }
            catch (Exception ex)
            {
                
                Logs.WriteErrorLog($"Error de conexión: {ex.Message}");
                
            }

            return Task.CompletedTask;

        }

        public void Disconnect()
        {
            if (_client.IsConnected)
            {
                _client.Disconnect();
            }
        }

        public async Task SendReportIngesta()
        {

            var sarlaftReportLogs = await _reportExcel.CreateSarlaftReportLogs();
            var sarlaftReport = await _reportExcel.CreateSarlaftReport();
            var emailNameList = _settingRepository.GetEmailNames();

            foreach (var emailName in emailNameList)
            {
                var sendEmail = new SendGridEmailDto
                {
                    email = emailName,
                    From = _configuration.GetSection("SendGridEmailSender:From").Value,
                    Key = _configuration.GetSection("SendGridEmailSender:Key").Value,
                    Name = _configuration.GetSection("SendGridEmailSender:Name").Value,
                    TemplateId = _configuration.GetSection("SendGridEmailSender:IngestaEmail").Value,
                    Data = new
                    {
                        dateingesta = DateTime.UtcNow.ToString("dd/MM/yyyy")
                    },
                    Attachments = new List<AttachmentDto>
                    {
                        new AttachmentDto
                        {
                            Filename = "Ingesta con Logs de aceptacion BangK.xlsx",
                            Content = Convert.ToBase64String(sarlaftReportLogs),
                            Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                        },

                        new AttachmentDto
                        {
                            Filename = "Ingesta BangK.xlsx",
                            Content = Convert.ToBase64String(sarlaftReport),
                            Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                        }
                    }
                };

                await SendGridEmailSender.SendEmailAsync(sendEmail);
            }

        }
    }
    public class SftpConnectionException : Exception
    {
        public SftpConnectionException(string message) : base(message)
        {
        }

        
    }


}
