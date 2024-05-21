namespace adv_Backend_Entrance.AdminPanel.Models
{
    public class UsersFilterModel
    {
        public int page { get; set; }
        public int size { get; set; }
        public Guid userId { get; set; }
        public string? email { get; set; }
        public string? Lastname { get; set; }
        public string? Firstname { get; set; }
    }
}
