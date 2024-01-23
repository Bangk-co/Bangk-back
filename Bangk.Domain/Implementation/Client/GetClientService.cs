using Bangk.Common.Dto;
using Bangk.Domain.Contract.Costumer;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Bangk.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Bangk.Domain.Implementation.Client
{
    public class GetClientService : ICostumerService
    {

        private readonly IClientRepository _ClientRepository;

        public GetClientService(IClientRepository ClientRepository)
        {
            _ClientRepository = ClientRepository;
        }

        public async Task<PaginationDto<ClientDto>> Execute(int currentPage, int pageSize, string searchValue)
        {
            var query = _ClientRepository.GetClientListQuery(searchValue);

            return new PaginationDto<ClientDto>()
            {
                Items = await _ClientRepository.GetDtoList(query, pageSize, currentPage),
                TotalItems = _ClientRepository.GetTotalDtoRecords(query),
                CurrentPage = currentPage,
                PageSize = pageSize
            };
        }

        
    }
}
