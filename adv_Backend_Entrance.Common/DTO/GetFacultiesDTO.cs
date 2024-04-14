using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO
{
    public class GetFacultiesDTO
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        public string name { get; set; }
    }
}
