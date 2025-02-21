using Bangk.Common.Dto.MecharntsInfoDto;
using Bangk.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Integrations
{
    public interface IPaymentSourcesBancolombiaTransferService
    {
        Task<ResponseDto<PaymentWompiResponseDto>> Execute(string transactionId, int value);
    }
}
