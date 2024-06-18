using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanApplication;
using Bangk.Repositories.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.LoanApplication
{
    public class GetLoanApplicationFirstThreeService : IGetLoanApplicationFirstThreeService
    {
        private readonly IClientRepository _clientRepository;

        public GetLoanApplicationFirstThreeService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async  Task<List<ClientPolicyDto>> Execute(int clientId)
        {
            return await _clientRepository.GetLoanApplicationLastThreeByClient(clientId);
        }
    }
}
