using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.EntranceService.Applicant
{
    public class AddProgramsDTO
    {
        public Guid ApplicationId { get; set; }
        public List<SetProgramsPriorityDTO> Programs { get; set; }
    }
}
