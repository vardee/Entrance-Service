using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO
{
    public class GetProgramsWithPaginationDTO
    {
        public List<GetProgramsDTO> programs { get; set; }
        public PaginationInformation pagination { get; set; }
    }
}
