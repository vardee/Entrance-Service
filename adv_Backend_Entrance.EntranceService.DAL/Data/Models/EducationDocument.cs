using adv_Backend_Entrance.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.EntranceService.DAL.Data.Models
{
    public class EducationDocument
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public EducationLevel EducationLevel { get; set; }
    }
}
