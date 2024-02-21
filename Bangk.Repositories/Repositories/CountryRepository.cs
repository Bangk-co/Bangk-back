using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public class CountryRepository: GenericRepository<Country, CountryDto>, ICountryRepository
    {
        public CountryRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<List<CountryDto>> GetCountries()
        {
            return await Context.Countries.Select(c => new CountryDto
            {
                Id = c.Id,
                Name = c.Name
            }).OrderBy(x => x.Name).ToListAsync();
        }
    }
}
