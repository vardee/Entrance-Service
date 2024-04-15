using adv_Backend_Entrance.FacultyService.MVCPanel.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.FacultyService.DAL.Data.Models
{
    public class EducationDocumentTypeNextEducationLevel
    {
        public Guid EducationDocumentTypeId { get; set; }
        public EducationDocumentTypeModel EducationDocumentType { get; set; }

        public int EducationLevelId { get; set; }
        public string EducationLevelName { get; set; }
        public EducationLevelModel EducationLevel { get; set; }
    }
}
