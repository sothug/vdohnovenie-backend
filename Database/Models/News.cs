using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Models;

namespace backend.Database.Models;

public class News : IValidatableObject, IEntity
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Заголовок обязателен")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Заголовок должен быть от 3 до 100 символов")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Содержимое обязательно")]
    public string Content { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

    [Required(ErrorMessage = "Автор обязателен")]
    [ForeignKey("Author")]
    public int AuthorId { get; set; }
    public virtual User Author { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return new List<ValidationResult>();
    }
}