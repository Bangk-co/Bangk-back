using Bangk.Common.Dto;
using Bangk.Domain.Contract.Settings;
using Bangk.Repositories.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Settings
{
    public class GetSettingListService : IGetSettingListService
    {
        private readonly ISettingRepository _settingRepository;
        public GetSettingListService(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public async Task<List<SettingListDto>> Execute(int groupSettingId)
        {
            return await _settingRepository.GetSettingList(groupSettingId);
        }
    }
}
