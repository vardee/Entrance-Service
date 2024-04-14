using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO
{
    public class PaginationInformation
    {
        public int Page { get; set; } 
        public int Size { get; set; }
        public int Current { get; set; }
    }
}
