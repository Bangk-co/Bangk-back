
using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public interface ISettingRepository : IGenericRepository<Setting, SettingListDto>
    {
        Task<List<SettingListDto>> GetSettingList(int groupSettingId, string searchText);
    }
}
