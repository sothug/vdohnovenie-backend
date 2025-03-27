using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Models;

namespace backend.Database.Models;

public class Schedule : IValidatableObject, IEntity
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Тренер обязателен")]
    [ForeignKey("Coach")]
    public int CoachId { get; set; }
    public virtual User? Coach { get; set; }

    [Required(ErrorMessage = "Зал обязателен")]
    [ForeignKey("GymHall")]
    public int GymHallId { get; set; }
    public virtual GymHall? GymHall { get; set; }

    [Required(ErrorMessage = "Время начала обязательно")]
    public DateTime StartTime { get; set; }

    [Required(ErrorMessage = "Время окончания обязательно")]
    public DateTime EndTime { get; set; }

    [StringLength(200, ErrorMessage = "Описание не более 200 символов")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Тип занятия обязателен")]
    public bool IsGroupLesson { get; set; } // true - групповое, false - индивидуальное

    public decimal? IndividualPrice { get; set; } // Цена для индивидуального занятия, null для групповых

    // Связь многие-ко-многим с детьми (посещение занятий)
    public virtual ICollection<User> Attendees { get; set; } = new List<User>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (EndTime <= StartTime)
            results.Add(new ValidationResult("Время окончания должно быть позже времени начала", new[] { nameof(EndTime) }));

        if (!IsGroupLesson && Attendees.Count > 1)
            results.Add(new ValidationResult("Индивидуальное занятие может иметь только одного участника", new[] { nameof(Attendees) }));

        if (IndividualPrice.HasValue && IndividualPrice < 0)
            results.Add(new ValidationResult("Цена не может быть отрицательной", new[] { nameof(IndividualPrice) }));

        if (IsGroupLesson && IndividualPrice.HasValue)
            results.Add(new ValidationResult("Групповое занятие не должно иметь индивидуальную цену", new[] { nameof(IndividualPrice) }));

        // if (!IsGroupLesson && Attendees.Count == 0)
        //     results.Add(new ValidationResult("Индивидуальное занятие должно иметь хотя бы одного участника", new[] { nameof(Attendees) }));

        return results;
    }
}