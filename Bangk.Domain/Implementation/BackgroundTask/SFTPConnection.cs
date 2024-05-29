using Microsoft.Extensions.Hosting;
using Renci.SshNet;
using Renci.SshNet.Common;
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

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
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
                    client.Connect();
                    if (client.IsConnected)
                    {
                        Console.WriteLine("Conexión establecida correctamente.");
                    }
                    else
                    {
                        Console.WriteLine("No se pudo establecer la conexión.");
                        throw new SftpConnectionException("No se pudo establecer la conexión al servidor SFTP.");
                    }
                }
                catch (SshAuthenticationException ex)
                {
                    Console.WriteLine($"Error de autenticación: {ex.Message}");

                    throw;
                }
                finally
                {
                    client.Disconnect();
                }
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
