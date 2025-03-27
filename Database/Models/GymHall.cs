using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.Database.Models;

public class GymHall : IValidatableObject, IEntity
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Название зала обязательно")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Название зала должно быть от 2 до 50 символов")]
    public string Name { get; set; }

    [StringLength(200, ErrorMessage = "Описание не более 200 символов")]
    public string Description { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Вместимость должна быть положительной")]
    public int Capacity { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return new List<ValidationResult>();
    }
}