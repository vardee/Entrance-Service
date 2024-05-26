using adv_Backend_Entrance.Common.Enums;

namespace adv_Backend_Entrance.AdminPanel.Models
{
    public class EditApplicantPageView
    {
        public Guid CurrentManager { get; set; }
        public Guid Person { get; set; }
        public List<RoleType> Roles { get; set; }
        public EditApplicantProfileModel Profile { get; set; }
    }
}
