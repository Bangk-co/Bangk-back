using Bangk.Common.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Settings
{
    public interface IGetSettingListService
    {
        Task<List<SettingListDto>> Execute(int groupSettingId, string searchText);
    }
}
