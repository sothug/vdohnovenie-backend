using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.Database.Models;

public class SubscriptionType : IValidatableObject, IEntity
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Название абонемента обязательно")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Название должно быть от 3 до 100 символов")]
    public string Name { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Количество занятий должно быть положительным")]
    public int NumberOfClasses { get; set; }

    [Range(0.5, 24, ErrorMessage = "Длительность должна быть от 0.5 до 24 часов")]
    public double DurationHours { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Цена не может быть отрицательной")]
    public decimal Price { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return new List<ValidationResult>();
    }
}