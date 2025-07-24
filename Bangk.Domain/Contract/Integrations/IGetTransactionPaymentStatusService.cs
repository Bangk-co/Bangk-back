using Bangk.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Integrations
{
    public interface IGetTransactionPaymentStatusService
    {
        Task<ResponseDto<dynamic>> Execute(string id);
    }
}