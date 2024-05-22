using adv_Backend_Entrance.Common.Enums;

namespace adv_Backend_Entrance.AdminPanel.Models
{
    public class ManagerFilterModel
    {
        public int Size { get; set; } = 10;
        public int Page { get; set; } = 1;
        public RoleType? Role { get; set; } = null;
        public string? Name { get; set; } = "";
    }
}
