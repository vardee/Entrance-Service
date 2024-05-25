using adv_Backend_Entrance.Common.DTO.EntranceService.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.FacultyService
{
    public class GetAllQuerybleImportsDTO
    {
        public IQueryable<GetImprotsDTO> Imports { get; set; }
        public PaginationInformation PaginationInformation { get; set; }
    }
}
