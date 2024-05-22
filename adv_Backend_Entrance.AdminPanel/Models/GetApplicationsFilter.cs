using adv_Backend_Entrance.Common.Enums;

namespace adv_Backend_Entrance.AdminPanel.Models
{
    public class GetApplicationsFilter
    {
        public int size { get; set; } = 10;
        public int page { get; set; } = 1;
        public string? name { get; set; } = null;
        public Guid? ProgramId { get; set; } = null;
        public List<ProgramModel>? Programs { get; set; } = new List<ProgramModel>();
        public Guid? FacultyId { get; set; } = null;
        public List<FacultiesModel>? Faculties { get; set; } = new List<FacultiesModel>();
        public List<EntranceApplicationStatus>? entranceApplicationStatuses { get; set; } = new List<EntranceApplicationStatus>();
        public bool? haveManager { get; set; } = null;
        public bool? isMy { get; set; } = null;
        public Guid? managerId { get; set; } = null;
        public List<ManagerModel>? Managers { get; set; } = new List<ManagerModel>();
        public Sorting? timeSorting { get; set; } = Sorting.CreateAsc;
    }
}
