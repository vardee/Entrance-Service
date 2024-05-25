namespace adv_Backend_Entrance.AdminPanel.Models
{
    public class ProgramsModel
    {
        public Guid ProgramId { get; set; }
        public Guid ApplicationId { get; set; }
        public int Priority { get; set; }
        public string ProgramName { get; set; }
        public string FacultyName { get; set; }
        public Guid FacultyId { get; set; }
        public string ProgramCode { get; set; }
    }
}
