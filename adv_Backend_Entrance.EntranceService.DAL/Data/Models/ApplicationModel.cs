using adv_Backend_Entrance.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.EntranceService.DAL.Data.Models
{
    public class ApplicationModel
    {
        public Guid Id { get; set; }
        public Guid ApplicantId { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ChangedTime { get; set; }
        public EntranceApplicationStatus ApplicationStatus { get; set; }
        public Guid ManagerId { get; set; }
    }
}
