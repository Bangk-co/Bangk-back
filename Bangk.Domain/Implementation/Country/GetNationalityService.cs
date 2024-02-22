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
    public class GetNationalityService: IGetNationalityService
    {
        private readonly ICountryRepository _countryRepository;
        public GetNationalityService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<List<NationalityDto>> Execute()
        {
            return await _countryRepository.GetNationality();
        }
    }
}
