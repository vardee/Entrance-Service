using adv_Backend_Entrance.Common.Enums;

namespace adv_Backend_Entrance.Common.DTO.EntranceService.Manager
{
    public class GetAllManagersDTO
    {
        public Guid ManagerId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public RoleType Role { get; set; }
    }
}
