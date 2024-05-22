namespace adv_Backend_Entrance.AdminPanel.Models
{
    public class ManagersViewModel
    {
        public ManagerFilterModel Filter { get; set; }
        public List<ManagerModel> Manager { get; set; } 
        public Guid CurrentId { get; set; }
    }
}
