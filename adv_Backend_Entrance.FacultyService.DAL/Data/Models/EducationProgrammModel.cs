namespace adv_Backend_Entrance.FacultyService.MVCPanel.Data.Models
{
    public class EducationProgrammModel
    {
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Language { get; set; }
        public string EducationForm { get; set; }
        public Guid FacultyId { get; set; }
        public int EducationLevelId { get; set; }
    }
}
