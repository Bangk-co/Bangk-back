
using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public interface IGroupSettingRepository : IGenericRepository<GroupSetting, GroupSettingListDto>
    {
        Task<List<GroupSettingListDto>> GetGroupSettingList();
    }
}
