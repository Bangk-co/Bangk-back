using Bangk.Common.Dto;
using Bangk.Domain.Contract.Client;
using Bangk.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Client
{
    public class GetClientPolicyService: IGetClientPolicyService
    {
        private readonly IClientRepository _clientRepository;

        public GetClientPolicyService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<List<ClientPolicyDto>> Execute(int clientId)
        {
            var query =  _clientRepository.GetPolicyClient(clientId);

            return query.ToList();
        }
    }
}
