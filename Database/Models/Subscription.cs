using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Models;

namespace backend.Database.Models;

public class Subscription : IValidatableObject, IEntity
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Пользователь обязателен")]
    [ForeignKey("User")]
    public int UserId { get; set; }
    public virtual User User { get; set; }

    [Required(ErrorMessage = "Тип абонемента обязателен")]
    [ForeignKey("SubscriptionType")]
    public int SubscriptionTypeId { get; set; }
    public virtual SubscriptionType SubscriptionType { get; set; }

    [Required(ErrorMessage = "Дата покупки обязательна")]
    public DateTime PurchaseDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Остаток занятий не может быть отрицательным")]
    public int RemainingClasses { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (ExpirationDate.HasValue && ExpirationDate < PurchaseDate)
            results.Add(new ValidationResult("Дата истечения не может быть раньше даты покупки", new[] { nameof(ExpirationDate) }));

        return results;
    }
}