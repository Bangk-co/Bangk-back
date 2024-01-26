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
    public class UpdateClientService : IUpdateClientService
    {
        private readonly IClientRepository _ClientRepository;
        
        public UpdateClientService(IClientRepository ClientRepository)
        {
            _ClientRepository = ClientRepository;
        }
       
        public async Task<ResponseDto<ClientUpdateInfoDto>> UpdateClient(int clientId, ClientUpdateInfoDto updateClient)
        {
            var result = new ResponseDto<ClientUpdateInfoDto>();
            var existClient =  _ClientRepository.ClientByIdUpdate(clientId, updateClient);
            
            if (existClient == null)
            {
                result.HasErrors = true;
                result.Errors.Add("Cliente no encontrado");
                return result;
                
            }
            else
            {
                result.HasErrors = false;
                result.Result = existClient.Result;
                return result;
            }
            

        }
    }
}
