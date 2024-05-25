using adv_Backend_Entrance.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.FacultyService
{
    public class GetImprotsDTO
    {
        public ImportType Type { get; set; }
        public ImportStatus Status { get; set; }
        public DateTime ImportWas {  get; set; }
    }
}
