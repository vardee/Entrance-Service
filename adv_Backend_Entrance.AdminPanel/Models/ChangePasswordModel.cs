using System.ComponentModel.DataAnnotations;

namespace adv_Backend_Entrance.AdminPanel.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Старый пароль обязателен для заполнения")]
        [DataType(DataType.Password)]
        [EmailAddress]
        [Display(Name = "Старый пароль")]
        public string OldPassword { get; set; } = "";
        
        [Required(ErrorMessage = "Пароль обязателен для заполнения")]
        [DataType(DataType.Password)]
        [EmailAddress]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Пароль обязателен для заполнения")]
        [DataType(DataType.Password)]
        [Display(Name = "Повторный пароль")]
        public string ConfirmPassword { get; set; } = "";
    }
}
