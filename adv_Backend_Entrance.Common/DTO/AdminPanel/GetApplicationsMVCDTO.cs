using adv_Backend_Entrance.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.AdminPanel
{
    public class GetApplicationsMVCDTO
    {
        public int size { get; set; }
        public int page { get; set; }
        public string? name { get; set; }
        public Guid? ProgramId { get; set; }
        public List<Guid>? Faculties { get; set; }
        public List<EntranceApplicationStatus>? entranceApplicationStatuses { get; set; }
        public bool? haveManager { get; set; }
        public bool? isMy { get; set; }
        public Guid? managerId { get; set; }
        public Sorting? timeSorting { get; set; }
    }
}
