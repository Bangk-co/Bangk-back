using Bangk.Api.Extensions;
using Bangk.Common.Dto;
using Bangk.Domain.Contract;
using Bangk.Domain.Contract.Sftp;
using Bangk.Domain.Implementation.BackgroundTask;
using Bangk.Repositories.Context;
using Coravel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Globalization;
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;

namespace Bangk.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpContextAccessor();

            // Swagger Documentation
            services.AddSwaggerDocumentation();

            // Set configuration for Entity Framework
            services.AddDbContext<AppDbContext>
                (options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);
            
            services.AddCors();

            // Add JwtAuthentication
            services.AddJwtAuthentication(Configuration.GetSection("Jwt:Token").Value);

            // Register Services
            services.RegisterServices();

            // Add converter to DI
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            services.Configure<SftpConnectionOptionsDto>(Configuration.GetSection("SftpConnection"));

            services.AddSingleton(provider =>
            {
                var sftpOptions = provider.GetRequiredService<IOptions<SftpConnectionOptionsDto>>().Value;
                var sftpClient = new SftpClient(sftpOptions.Host, sftpOptions.Port, sftpOptions.Username, sftpOptions.Password);
                return sftpClient;
            });

            services.AddScheduler();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            // Configuracion de cultura 
            var cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;


            // Configuración Logger
            var columnOpts = new ColumnOptions();
            columnOpts.Store.Remove(StandardColumn.MessageTemplate);
            columnOpts.Store.Remove(StandardColumn.Properties);
            columnOpts.PrimaryKey = columnOpts.Id;
            columnOpts.TimeStamp.NonClusteredIndex = true;
            Log.Logger = new LoggerConfiguration()
                .WriteTo
                .MSSqlServer(
                connectionString: Configuration.GetConnectionString("DefaultConnection"),
                columnOptions: columnOpts,
                sinkOptions: new MSSqlServerSinkOptions { TableName = "Logs" })
                .CreateLogger();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            // Configure Swagger
            app.AddCustomSwagger();

            // Global Error Handling
            app.ConfigureExceptionHandler();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            serviceProvider.UseScheduler(scheduler =>
            {
                //scheduler.Schedule<ReportSarlaftJob>().DailyAt(22, 0)
                //.Monday()
                //.Tuesday()
                //.Wednesday()
                //.Thursday()
                //.Friday().PreventOverlapping(nameof(ReportSarlaftJob)); 
                //scheduler.Schedule<ReportSarlaftJob>().DailyAt(17, 0)
                //.Saturday().PreventOverlapping(nameof(ReportSarlaftJob));
                scheduler.Schedule<RetryIntegrationsJob>().EveryMinute().PreventOverlapping(nameof(RetryIntegrationsJob));
                scheduler.Schedule<CompleteApplicationJob>().EveryFiveMinutes().PreventOverlapping(nameof(CompleteApplicationJob));
            });
        }
    }
}
