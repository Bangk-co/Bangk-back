using Bangk.Common.Dto;
using Bangk.Domain.Contract.Country;
using Bangk.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Country
{
    public class GetCountryService: IGetCountryService
    {
        private readonly ICountryRepository _countryRepository;
        public GetCountryService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<List<CountryDto>> Execute()
        {
            return await _countryRepository.GetCountries();
        }
    }
}
