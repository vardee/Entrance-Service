using adv_Backend_Entrance.Common.Enums;
using System.ComponentModel.DataAnnotations;

public class UserRegisterDTO
{
    [Required]
    public string FullName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Patronymic { get; set; }
    [Required]
    public Gender Gender { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Phone { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string ConfirmPassword { get; set; }

    public DateTime BirthDate { get; set; }
}
