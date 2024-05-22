namespace adv_Backend_Entrance.AdminPanel.Models
{
    public class UsersFilterModel
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 5;
        public string? Email { get; set; } = "";
        public string? Lastname { get; set; } = "";
        public string? Firstname { get; set; } = "";
    }
}
