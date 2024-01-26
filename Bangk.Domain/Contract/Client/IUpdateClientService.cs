using Bangk.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Client
{
    public interface IUpdateClientService
    {
        Task<ResponseDto<ClientUpdateInfoDto>> UpdateClient(int clientId, ClientUpdateInfoDto updateClient);
    }
}
