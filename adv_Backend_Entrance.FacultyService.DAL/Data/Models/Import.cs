using adv_Backend_Entrance.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.FacultyService.DAL.Data.Models
{
    public class Import
    {
        public Guid Id { get; set; }
        public ImportStatus Status { get; set; }
    }
}
