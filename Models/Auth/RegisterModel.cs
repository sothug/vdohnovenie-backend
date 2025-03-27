using System.ComponentModel.DataAnnotations;

namespace backend.Models.Auth;

public class RegisterModel
{
    [Required(ErrorMessage = "Username обязателен")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username должен быть от 3 до 50 символов")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Пароль обязателен")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Полное имя обязательно")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Полное имя должно быть от 2 до 100 символов")]
    public string FullName { get; set; }

    public DateTime? DateOfBirth { get; set; }
    public int? ParentId { get; set; } // Для детей
}