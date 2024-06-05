using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Sftp
{
    public interface ISFTPConnection
    {
        Task ConnectAsync();
        void Disconnect();
    }
}
