using Bangk.Domain.Contract;
using Coravel.Invocable;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.BackgroundTask
{
    public class ReportSarlaftJob : IInvocable
    {
        private readonly IServiceProvider _serviceProvider;

        public ReportSarlaftJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Invoke()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var sftpConnection = scopedServices.GetRequiredService<SFTPConnection>();
                    await sftpConnection.BangkConnection(CancellationToken.None);

                    var reportExcel = scopedServices.GetRequiredService<IReportExcel>();
                    await reportExcel.CreateSarlaftReportLogs();
                    await reportExcel.CreateSarlaftReport();
                }
            }
            catch (SftpConnectionException ex)
            {
                Console.WriteLine($"Error al establecer conexión con el servidor SFTP: {ex.Message}");
            }
        }
    }
}
