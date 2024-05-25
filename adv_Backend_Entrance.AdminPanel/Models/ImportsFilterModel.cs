using adv_Backend_Entrance.Common.Enums;

namespace adv_Backend_Entrance.AdminPanel.Models
{
    public class ImportsFilterModel
    {
        public int size { get; set; } = 10;
        public List<ImportType> Types { get; set; } = new List<ImportType>();
    }
}
