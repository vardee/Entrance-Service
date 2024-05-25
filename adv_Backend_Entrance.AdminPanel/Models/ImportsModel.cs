using adv_Backend_Entrance.Common.Enums;

namespace adv_Backend_Entrance.AdminPanel.Models
{
    public class ImportsModel
    {
        public ImportType Type { get; set; }
        public ImportStatus Status { get; set; }
        public DateTime ImportWas { get; set; }
    }
}
