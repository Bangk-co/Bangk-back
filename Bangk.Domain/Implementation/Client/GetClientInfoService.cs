using Bangk.Common.Dto;
using Bangk.Domain.Contract.Costumer;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Client
{
    public class GetClientInfoService : IClientInfoService
    {
        private readonly IClientRepository _IClientRepository;

        public GetClientInfoService(IClientRepository clientRepository)
        {
            _IClientRepository = clientRepository;
        }

        public async Task<List<ClientInfoDto>> Execute(int clientId)
        {
            var query = _IClientRepository.GetClientByIdInfo(clientId);

            return query.ToList();
        }

        
    }
}
