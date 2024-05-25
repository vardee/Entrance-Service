using adv_Backend_Entrance.Common.Enums;

namespace adv_Backend_Entrance.AdminPanel.Models
{
    public class UsersPageViewModel
    {
        public List<UsersModel> Users { get; set; }
        public UsersFilterModel Filters { get; set; }
        public Guid CurrentId { get; set; }
        public List<RoleType> Roles { get; set; }
    }
}
