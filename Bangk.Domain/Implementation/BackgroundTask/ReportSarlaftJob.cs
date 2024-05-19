using Bangk.Domain.Contract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.BackgroundTask
{
    public class ReportSarlaftJob
    {
        private readonly IServiceProvider _serviceProvider;

        public ReportSarlaftJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Execute()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var reportExcel = scopedServices.GetRequiredService<IReportExcel>();

                reportExcel.CreateSarlaftReportLogs().Wait();
                reportExcel.CreateSarlaftReport().Wait();
            }
        }
    }

}
