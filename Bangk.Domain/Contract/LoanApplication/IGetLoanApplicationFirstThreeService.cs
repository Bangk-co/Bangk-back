using Bangk.Common.Dto;
using System.Collections.Generic;

namespace Bangk.Domain.Contract.LoanApplication
{
    public interface IGetLoanApplicationFirstThreeService
    {
        List<ClientPolicyDto> Execute(int clientId);
    }
}
