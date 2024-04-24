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
    public class ReportSarlaftJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public ReportSarlaftJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            var currentTime = DateTime.Now;
            var nextRun = GetNextRunTime(currentTime);
            var delay = nextRun - currentTime;

            
            _timer = new Timer(DoWork, null, delay, TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var reportExcel = scopedServices.GetRequiredService<IReportExcel>();

                
                reportExcel.CreateSarlaftReport();
            }
        }

        private DateTime GetNextRunTime(DateTime currentTime)
        {
            
            var nextRun = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 17, 0, 0); 

            if (currentTime.DayOfWeek == DayOfWeek.Saturday)
            {
                nextRun = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 12, 0, 0); 
            }
            else if (currentTime.Hour >= 18 || (currentTime.Hour == 17 && currentTime.Minute >= 30))
            {
                
                nextRun = nextRun.AddDays(1);
            }

            return nextRun;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }

    }
}
