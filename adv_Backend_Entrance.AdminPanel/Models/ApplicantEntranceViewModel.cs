namespace adv_Backend_Entrance.AdminPanel.Models
{
    public class ApplicantEntranceViewModel
    {
        public Guid Id { get; set; }
        public PassportModel PassportModel { get; set; }
        public EducationDocumentModel EducationDocumentModel { get; set; }
        public List<ProgramsModel> Programs { get; set; }
    }
}
