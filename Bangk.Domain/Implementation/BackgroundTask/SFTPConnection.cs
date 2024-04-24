using Microsoft.Extensions.Hosting;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.BackgroundTask
{
    public class SFTPConnection : BackgroundService
    {
        

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await BangkConnection(stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                
                // Espera un tiempo antes de volver a intentar la conexión
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        public async Task BangkConnection(CancellationToken cancellationToken)
        {
            string host = "mfxsftp.mfxservices.com";
            string username = "SBS_BANGK_RW";
            string password = "Lk@13HgYT";
            int port = 22;

            using (var client = new SftpClient(host, port, username, password))
            {
                try
                {
                    await Task.Run(() => client.Connect());
                    if (client.IsConnected)
                    {
                        Console.WriteLine("Conexión establecida correctamente.");
                        // Aquí puedes realizar otras operaciones relacionadas con la conexión
                    }
                    else
                    {
                        Console.WriteLine("No se pudo establecer la conexión.");
                    }
                }
                finally
                {
                    client.Disconnect();
                }
            }
        }

    }
}
