using System.ComponentModel.DataAnnotations.Schema;

namespace Bangk.Repositories.Entities
{
    public class Setting : EntityBase
    {
        public string Name { get; set; }
        public string Code { get; set; }
        [ForeignKey("SettingGroup")]
        public int SettingGroupId { get; set; }
        public string IconsBanksUrl { get; set; }
        public GroupSetting SettingGroup { get; set; }
    }
}
