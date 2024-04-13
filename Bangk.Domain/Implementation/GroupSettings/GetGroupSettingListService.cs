using Bangk.Common.Dto;
using Bangk.Domain.Contract.GroupSettings;
using Bangk.Repositories.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.GroupSettings
{
    public class GetGroupSettingListService : IGetGroupSettingListService
    {
        private readonly IGroupSettingRepository _groupSettingRepositorycs;
        public GetGroupSettingListService(IGroupSettingRepository groupSettingRepositorycs)
        {
            _groupSettingRepositorycs = groupSettingRepositorycs;
        }

        public async Task<List<GroupSettingListDto>> Execute()
        {
            return await _groupSettingRepositorycs.GetGroupSettingList();
        }
    }
}
