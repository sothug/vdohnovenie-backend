using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Models;

namespace backend.Database.Models;

public class User : IValidatableObject, IEntity
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Username обязателен")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username должен быть от 3 до 50 символов")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Пароль обязателен")]
    public string PasswordHash { get; set; }

    [Required(ErrorMessage = "Роль обязательна")]
    [RegularExpression("^(Parent|Child|Coach|Admin)$", ErrorMessage = "Роль должна быть: Parent, Child, Coach или Admin")]
    public string Role { get; set; }

    [Required(ErrorMessage = "Полное имя обязательно")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Полное имя должно быть от 2 до 100 символов")]
    public string FullName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("Parent")]
    public int? ParentId { get; set; }
    public virtual User? Parent { get; set; }
    public virtual ICollection<User> Children { get; set; } = new List<User>();

    // Абонементы только для групповых занятий детей
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    // Расписание для тренеров (проведение) и детей (посещение)
    [InverseProperty("Coach")]
    public virtual ICollection<Schedule> SchedulesLed { get; set; } = new List<Schedule>(); // Для тренеров
    public virtual ICollection<Schedule> SchedulesAttended { get; set; } = new List<Schedule>(); // Для детей
    // Новости для админов/тренеров
    public virtual ICollection<News> News { get; set; } = new List<News>();
    // Галерея для админов/тренеров
    public virtual ICollection<GalleryItem> GalleryItems { get; set; } = new List<GalleryItem>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (Role == "Child" && !ParentId.HasValue)
            results.Add(new ValidationResult("У ребенка должен быть указан родитель", new[] { nameof(ParentId) }));

        if (Role == "Child" && !DateOfBirth.HasValue)
            results.Add(new ValidationResult("Дата рождения обязательна для ребенка", new[] { nameof(DateOfBirth) }));

        if (ParentId.HasValue && ParentId == Id)
            results.Add(new ValidationResult("Пользователь не может быть своим собственным родителем", new[] { nameof(ParentId) }));

        return results;
    }
}