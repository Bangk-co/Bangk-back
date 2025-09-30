using Bangk.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Integrations
{
    public interface IGenerateTokenTrxBancolombiaService
    {
        Task<ResponseDto<Bangk.Common.Dto.BancolombiaButtonTransferResponseDto.BancolombiaButtonTransferResponseDto>> Execute(int applicationId, bool isMed);
    }
}
