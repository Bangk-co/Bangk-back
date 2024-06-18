using Bangk.Common.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.LoanApplication
{
    public interface IGetLoanApplicationFirstThreeService
    {
        Task<List<ClientPolicyDto>> Execute(int clientId);
    }
}
