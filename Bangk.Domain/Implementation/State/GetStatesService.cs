using Bangk.Common.Dto;
using Bangk.Domain.Contract.State;
using Bangk.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.State
{
    public class GetStatesService: IGetStatesService
    {
        private readonly IStateRepository _stateRepository;
        

        public GetStatesService (IStateRepository stateRepository)
        {
            _stateRepository = stateRepository;
        }

        public async Task<List<StateDto>> Execute(int countryId)
        {
            return await _stateRepository.GetByIdState(countryId);
        }
    }
}
