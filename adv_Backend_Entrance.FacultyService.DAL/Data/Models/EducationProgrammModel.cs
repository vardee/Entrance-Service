using adv_Backend_Entrance.Common.Enums;

namespace adv_Backend_Entrance.FacultyService.MVCPanel.Data.Models
{
    public class EducationProgrammModel
    {
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Language { get; set; }
        public EducationLanguage LanguageEnum { get; set; }
        public string EducationForm { get; set; }
        public EducationForm EducationFormEnum { get; set; }
        public Guid FacultyId { get; set; }
        public int EducationLevelId { get; set; }
        public EducationLevel EducationLevelEnum { get; set;}
        public string FacultyName { get; set; }
        public DateTime FacultyCreateTime { get; set; }
        public string EducationLevelName { get; set; }
    }
}
