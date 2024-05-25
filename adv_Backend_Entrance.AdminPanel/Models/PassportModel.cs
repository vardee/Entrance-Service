namespace adv_Backend_Entrance.AdminPanel.Models
{
    public class PassportModel
    {
        public int PassportNumber { get; set; }
        public string BirthPlace { get; set; }
        public DateOnly IssuedWhen { get; set; }
        public string IssuedWhom { get; set; }
    }
}
