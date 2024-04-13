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
    public class StateRepository: GenericRepository<State, StateDto>, IStateRepository
    {
        public StateRepository(AppDbContext context): base(context)
        {

        }

        public async Task<List<StateDto>> GetByIdState(int countryId)
        {
            return await Context.State.Where(s => s.CountryId == countryId).Select(s => new StateDto
            {
                Id = s.Id,
                Name = s.Name
                
            }).OrderBy(c => c.Name).ToListAsync();
        }
    }
}
