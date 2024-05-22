using adv_Backend_Entrance.Common.Enums;

namespace adv_Backend_Entrance.AdminPanel.Models
{
    public class ManagerModel
    {
        public Guid ManagerId {  get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public RoleType Role {  get; set; } 
    }
}
