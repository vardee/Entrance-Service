using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.EntranceService.Manager
{
    public class GetAllQuerybleManagersDTO
    {
        public List<GetAllManagersDTO> Managers { get; set; }
        public PaginationInformation PaginationInformation { get; set; }
    }
}
