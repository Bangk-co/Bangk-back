using Bangk.Business.Extension.SenderGrid;
using Bangk.Common;
using Bangk.Common.Dto;
using Bangk.Domain.Contract;
using Bangk.Domain.Contract.Sftp;
using Bangk.Repositories.Repositories;
using Coravel.Invocable;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.BackgroundTask
{
    public class ReportSarlaftJob : IInvocable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISFTPConnection _sftpConnection;
        private readonly IConfiguration _configuration;
        private readonly ISettingRepository _settingRepository;
        private readonly IReportExcel _reportExcel;


        public ReportSarlaftJob(IServiceProvider serviceProvider, ISFTPConnection sftpConnection, IConfiguration configuration, ISettingRepository settingRepository, IReportExcel reportExcel)
        {
            _serviceProvider = serviceProvider;
            _sftpConnection = sftpConnection;
            _configuration = configuration;
            _settingRepository = settingRepository;
            _reportExcel = reportExcel;
        }

        public async Task Invoke()
        {
            Logs.WriteInfoLog($"Running Scheduled Task: ReportSarlaftJob");
            await Execute();
        }

        public async Task Execute()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    await _sftpConnection.ConnectAsync();

                    await _sftpConnection.SendReportIngesta();

                    _sftpConnection.Disconnect();


                }
            }
            catch (SftpConnectionException ex)
            {
                Logs.WriteErrorLog($"Error al establecer conexión con el servidor SFTP: {ex.Message}");
            }
        }



    }
}
