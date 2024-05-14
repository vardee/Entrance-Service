using adv_Backend_Entrance.Common.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.EntranceService.Applicant
{
    public class CreateApplicationDTO
    {
        public int PassportId { get; set; }
        public Guid EducationId { get; set; }
        public List<SetProgramsPriorityDTO> ProgramsPriority { get; set; }
    }
}
