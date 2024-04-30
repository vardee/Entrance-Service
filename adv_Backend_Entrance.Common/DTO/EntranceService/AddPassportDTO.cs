using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.EntranceService
{
    public class AddPassportDTO
    {
        public int PassportNumber { get; set; }
        public string BirthPlace { get; set; }
        public DateOnly IssuedWhen { get; set; }
        public string IssuedWhom { get; set; }
    }
}
