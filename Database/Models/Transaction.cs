using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Models;

namespace backend.Database.Models;

public class Transaction : IValidatableObject, IEntity
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Subscription")]
    public int? SubscriptionId { get; set; }
    public virtual Subscription Subscription { get; set; }

    [ForeignKey("Schedule")]
    public int? ScheduleId { get; set; } // Для индивидуальных занятий
    public virtual Schedule Schedule { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Сумма не может быть отрицательной")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Дата оплаты обязательна")]
    public DateTime PaymentDate { get; set; }

    [Required(ErrorMessage = "Метод оплаты обязателен")]
    [StringLength(50, ErrorMessage = "Метод оплаты не более 50 символов")]
    public string PaymentMethod { get; set; }

    [Required(ErrorMessage = "Статус обязателен")]
    [RegularExpression("^(Pending|Completed|Failed)$", ErrorMessage = "Статус должен быть: Pending, Completed или Failed")]
    public string Status { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (!SubscriptionId.HasValue && !ScheduleId.HasValue)
            results.Add(new ValidationResult("Транзакция должна быть связана либо с абонементом, либо с занятием", 
                new[] { nameof(SubscriptionId), nameof(ScheduleId) }));

        if (SubscriptionId.HasValue && ScheduleId.HasValue)
            results.Add(new ValidationResult("Транзакция не может быть связана одновременно с абонементом и занятием", 
                new[] { nameof(SubscriptionId), nameof(ScheduleId) }));

        return results;
    }
}