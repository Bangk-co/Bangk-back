
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

        public async Task<List<SettingListDto>> GetSettingList(int groupSettingId, string searchText)
        {
            IQueryable<Setting> query = Context.Settings.Where(s => s.SettingGroupId == groupSettingId);

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(s => s.Code.Contains(searchText) || s.Name.Contains(searchText));
            }

            return await query.Select(s => new SettingListDto
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                GroupSettingId = s.SettingGroupId

            }).OrderBy(x => x.Name).ToListAsync();

        }

        public List<string> GetEmailNames()
        {
            int settingGroupId = 19;
            var emailNames = from setting in Context.Settings
                             where setting.SettingGroupId == settingGroupId
                             select setting.Name;
            return emailNames.ToList();
        }
    }
}
