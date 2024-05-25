using adv_Backend_Entrance.Common.Enums;

namespace adv_Backend_Entrance.AdminPanel.Models
{
    public class ApplicationModel
    {
        public Guid ManagerId { get; set; }
        public Guid ApplicationId { get; set; }
        public string ApplicantFullName { get; set; }
        public EntranceApplicationStatus ApplicationStatus { get; set; }
        public string ManagerEmail { get; set; }
        public Guid ApplicantId { get; set; }

        public List<ManagerModel> ManagerModels { get; set; }
    }
}
