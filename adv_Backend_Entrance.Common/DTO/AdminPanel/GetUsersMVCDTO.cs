using adv_Backend_Entrance.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.AdminPanel
{
    public class GetUsersMVCDTO
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public string? Email { get; set; }
        public string? Lastname { get; set; }
        public string? Firstname { get; set; }
    }
}
