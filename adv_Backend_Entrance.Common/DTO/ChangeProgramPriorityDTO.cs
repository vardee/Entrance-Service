using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO
{
    public class ChangeProgramPriorityDTO
    {
        public int ProgramPriority { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid ProgramId { get; set; } 
    }
}
