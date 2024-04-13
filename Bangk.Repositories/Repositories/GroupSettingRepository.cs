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
    public class GroupSettingRepository : GenericRepository<GroupSetting, GroupSettingListDto>, IGroupSettingRepository
    {
        public GroupSettingRepository(AppDbContext context) : base(context)
        {
           
        }

        public async Task<List<GroupSettingListDto>> GetGroupSettingList()
        {
            return await Context.GroupSettings.Select(g => new GroupSettingListDto
            {
                Id = g.Id,
                Name = g.Name
            }).OrderBy(x => x.Name).ToListAsync(); 
        }
    }
}
