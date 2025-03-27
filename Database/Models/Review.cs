using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.Database.Models;

public class Review : IEntity, IValidatableObject
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public string Text { get; set; }
    public int Rating { get; set; } // От 1 до 5
    public DateTime CreatedAt { get; set; }
    public int? ScheduleId { get; set; } // Опционально, если отзыв о конкретном занятии
    public int? CoachId { get; set; }   // Опционально, если отзыв о тренере

    public virtual User Author { get; set; }
    public virtual Schedule Schedule { get; set; }
    public virtual User Coach { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Text))
            yield return new ValidationResult("Текст отзыва обязателен", new[] { nameof(Text) });
        if (Rating < 1 || Rating > 5)
            yield return new ValidationResult("Рейтинг должен быть от 1 до 5", new[] { nameof(Rating) });
    }
}