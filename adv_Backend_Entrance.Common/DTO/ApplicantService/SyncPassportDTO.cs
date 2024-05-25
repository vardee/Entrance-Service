using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.DTO.ApplicantService
{
    public class SyncPassportDTO
    {
        public Guid UserId { get; set; }
        public Guid PassportId { get; set; }
        public int passportNumber { get; set; }
        public string birthPlace { get; set; }
        public DateOnly issuedWhen { get; set; }
        public string issuedWhom { get; set; }
    }
}
