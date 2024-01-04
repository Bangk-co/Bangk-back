using System.ComponentModel.DataAnnotations.Schema;

namespace Bangk.Repositories.Entities
{
    public class Setting : EntityBase
    {
        public string Name { get; set; }
        public string Code { get; set; }
        [ForeignKey("GroupSetting")]
        public int SettingGroupId { get; set; }
        public GroupSetting GroupSettings { get; set; }
    }
}
