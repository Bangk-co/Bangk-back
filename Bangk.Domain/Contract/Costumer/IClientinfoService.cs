using Bangk.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Costumer
{
    public interface IClientInfoService
    {
        Task<List<ClientInfoDto>> Execute(int clientId);


    }
}
