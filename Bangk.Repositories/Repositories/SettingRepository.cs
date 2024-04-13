
using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public class SettingRepository : GenericRepository<Setting, SettingListDto>, ISettingRepository
    {
        public SettingRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<List<SettingListDto>> GetSettingList(int groupSettingId)
        {
            return await Context.Settings.Where(s => s.SettingGroupId == groupSettingId).Select(s => new SettingListDto
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                GroupSettingId = s.SettingGroupId
                
            }).OrderBy(x => x.Name).ToListAsync();
        }
    }
}
