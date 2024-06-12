using Bangk.Common.Dto;
using Bangk.Domain.Contract.City;
using Bangk.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.City
{
    public class GetCityService: IGetCityService
    {
        private readonly ICityRepository _cityRepository;


        public GetCityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public async Task<(List<CityDto>, string)> Execute(int stateId)
        {
            return await _cityRepository.GetByIdCities(stateId);
        }

    }
}
