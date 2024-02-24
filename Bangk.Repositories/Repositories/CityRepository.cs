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
    public class CityRepository: GenericRepository<City, CityDto>, ICityRepository
    {
        public CityRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<List<CityDto>> GetByIdCities(int stateId)
        {
            return await Context.Cities.Where(c => c.StateId == stateId).Select(c => new CityDto
            {
                Id = c.Id,
                Name = c.Name

            }).OrderBy(x => x.Name).ToListAsync();
        }

    }
}
