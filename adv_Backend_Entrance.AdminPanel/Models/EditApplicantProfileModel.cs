using adv_Backend_Entrance.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace adv_Backend_Entrance.AdminPanel.Models
{
    public class EditApplicantProfileModel
    {
        public Guid CurrentId { get; set; }
        public List<RoleType> Roles { get; set; }
        public Guid ApplicantManagerId {get; set;}
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
        [Display(Name = "Гражданство")]
        public string? Nationality { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Пол")]
        public Gender Gender { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Телефон")]
        public string? Phone { get; set; }


        [DataType(DataType.DateTime)]
        [Display(Name = "Дата рождения")]
        public DateTime BirthDate { get; set; }
    }
}
