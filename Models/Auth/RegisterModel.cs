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
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен быть от 6 до 100 символов")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Полное имя обязательно")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Полное имя должно быть от 2 до 100 символов")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Роль обязательна")]
    [RegularExpression("^(Parent|Child|Coach|Admin)$", ErrorMessage = "Роль должна быть: Parent, Child, Coach или Admin")]
    public string Role { get; set; } = "Parent";

    [Required(ErrorMessage = "Дата рождения обязательна для ребенка")]
    public DateTime? DateOfBirth { get; set; }

    public int? ParentId { get; set; }
}