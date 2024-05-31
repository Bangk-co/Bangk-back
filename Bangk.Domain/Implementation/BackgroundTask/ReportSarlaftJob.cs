using Bangk.Common;
using Bangk.Domain.Contract;
using Bangk.Domain.Contract.Sftp;
using Coravel.Invocable;
using Microsoft.Extensions.DependencyInjection;
using Renci.SshNet;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.BackgroundTask
{
    public class ReportSarlaftJob : IInvocable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISFTPConnection _sftpConnection;


        public ReportSarlaftJob(IServiceProvider serviceProvider, ISFTPConnection sftpConnection)
        {
            _serviceProvider = serviceProvider;
            _sftpConnection = sftpConnection;
        }

        public async Task Invoke()
        {
            Logs.WriteInfoLog($"Running Scheduled Task: ReportSarlaftJob");
            var cancellationToken = new CancellationTokenSource().Token;
            await Execute(cancellationToken);
        }

        public async Task Execute(CancellationToken cancellationToken)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    await _sftpConnection.ConnectAsync(cancellationToken);

                    var reportExcel = scopedServices.GetRequiredService<IReportExcel>();
                    await reportExcel.CreateSarlaftReportLogs();
                    await reportExcel.CreateSarlaftReport();
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
