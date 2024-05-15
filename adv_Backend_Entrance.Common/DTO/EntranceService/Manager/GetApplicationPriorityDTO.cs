using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.EntranceService.Manager
{
    public class GetApplicationPriorityDTO
    {
        public Guid ProgramId { get; set; }
        public Guid ApplicationId { get; set; }
        public int Priority { get; set; }
        public string ProgramName { get; set; }
        public string FacultyName { get; set; }
        public Guid FacultyId { get; set; }
        public string ProgramCode { get; set; }
    }
}
