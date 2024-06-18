using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanApplication;
using Bangk.Repositories.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Bangk.Domain.Implementation.LoanApplication
{
    public class GetLoanApplicationFirstThreeService : IGetLoanApplicationFirstThreeService
    {
        private readonly IClientRepository _clientRepository;

        public GetLoanApplicationFirstThreeService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public List<ClientPolicyDto> Execute(int clientId)
        {
            var query = _clientRepository.GetPolicyClient(clientId);
            return query.OrderByDescending(q => q.CreationDateLoanApplication).Take(3).ToList();
        }
    }
}
