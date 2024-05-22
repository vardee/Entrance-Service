using adv_Backend_Entrance.Common.Enums;

namespace adv_Backend_Entrance.AdminPanel.Models
{
    public class UsersModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public List<RoleType> Roles { get; set; }
    }
}
