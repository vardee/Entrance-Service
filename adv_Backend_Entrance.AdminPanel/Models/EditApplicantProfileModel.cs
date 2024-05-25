using adv_Backend_Entrance.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace adv_Backend_Entrance.AdminPanel.Models
{
    public class EditApplicantProfileModel
    {
        public Guid Id { get; set; }
        [DataType(DataType.Text)]
        [Display(Name = "Имя")]
        public string? FirstName { get; set; }


        [DataType(DataType.Text)]
        [Display(Name = "Фамилия")]
        public string? LastName { get; set; }


        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }


        [DataType(DataType.Text)]
        [Display(Name = "Отчество")]
        public string? Patronymic { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Пол")]
        public Gender Gender { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Phone]
        [Display(Name = "Телефон")]
        public string? Phone { get; set; }


        [DataType(DataType.DateTime)]
        [Display(Name = "Дата рождения")]
        public DateTime BirthDate { get; set; }
    }
}
