using System.ComponentModel.DataAnnotations;

namespace adv_Backend_Entrance.AdminPanel.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email обязателен для заполнения")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Пароль обязателен для заполнения")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = "";
    }
}
