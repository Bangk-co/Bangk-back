using Bangk.Common;
using Bangk.Domain.Contract.Sftp;
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
    public class SFTPConnection : ISFTPConnection
    {
        private readonly SftpClient _client;

        public SFTPConnection(SftpClient client)
        {
            _client = client;
        }

        public Task ConnectAsync(CancellationToken cancellationToken)
        {
            try
            {
                _client.Connect();
                if (_client.IsConnected)
                {
                    Logs.WriteInfoLog("Conexión establecida correctamente.");
                }
                else
                {
                    throw new SftpConnectionException("No se pudo establecer la conexión al servidor SFTP.");
                }
            }
            catch (SshAuthenticationException ex)
            {
                Logs.WriteErrorLog($"Error de autenticación: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Logs.WriteErrorLog($"Error de conexión: {ex.Message}");
                throw;
            }

            return Task.CompletedTask;
        }

        public void Disconnect()
        {
            if (_client.IsConnected)
            {
                _client.Disconnect();
                Logs.WriteInfoLog("Conexión cerrada correctamente.");
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
