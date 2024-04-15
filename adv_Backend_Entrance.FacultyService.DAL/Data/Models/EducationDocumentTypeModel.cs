using adv_Backend_Entrance.Common.Enums;
using System;
using System.Collections.Generic;

namespace adv_Backend_Entrance.FacultyService.MVCPanel.Data.Models
{
    public class EducationDocumentTypeModel
    {
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string Name { get; set; }
        public int EducationLevelId { get; set; }
        public string EducationLevelName { get; set; }
        public EducationLevel EducationLevelEnum { get; set; }
    }
}
