using Bangk.Common.Dto;
using Bangk.Common.Dto.MecharntsInfoDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Integrations
{
    public interface IGetMerchantsTokenService
    {
        Task<ResponseDto<MecharntsInfoDto>> Execute();
    }
}