using Bangk.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.LoanApplication
{
    public interface ILoanApplicationServices
    {
        Task<List<LoanApplicationDto>> Execute(int ClientId);
    }
}
