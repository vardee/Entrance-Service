using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.UserService
{
    public class GetUsersPageDTO
    {
        public IQueryable<GetUsersDTO> Users { get; set; }
        public PaginationInformation Pagination { get; set; }
    }
}
