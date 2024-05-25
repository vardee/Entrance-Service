namespace adv_Backend_Entrance.AdminPanel.Models
{
    public class ImportsPageViewModel
    {
        public ImportsFilterModel Filter { get; set; }
        public ImportInformationModel ImportInformation { get; set; } = new ImportInformationModel();

        public List<ImportsModel> Imports { get; set; }
    }

}
