using Bangk.Common.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.GroupSettings
{
    public interface IGetGroupSettingListService
    {
        Task<List<GroupSettingListDto>> Execute();
    }
}
