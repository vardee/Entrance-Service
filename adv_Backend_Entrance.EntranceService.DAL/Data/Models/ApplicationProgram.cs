using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.EntranceService.DAL.Data.Models
{
    public class ApplicationProgram
    {
        public Guid ApplicationId { get; set; }
        public ApplicationModel Application { get; set; }
        public Guid ProgramId { get; set; }
        public string ProgramName { get; set; } 
        public int Priority { get; set; }
    }
}
