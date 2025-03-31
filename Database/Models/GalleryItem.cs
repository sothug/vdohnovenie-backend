using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Models;

namespace backend.Database.Models;

public class GalleryItem : IValidatableObject, IEntity
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "URL изображения обязателен")]
    [StringLength(500, ErrorMessage = "URL не более 500 символов")]
    public string ImageUrl { get; set; }

    [StringLength(200, ErrorMessage = "Описание не более 200 символов")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Дата загрузки обязательна")]
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    [Required(ErrorMessage = "Автор обязателен")]
    [ForeignKey("Author")]
    public int AuthorId { get; set; }
    public virtual User? Author { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (!Uri.TryCreate(ImageUrl, UriKind.Absolute, out _))
            results.Add(new ValidationResult("URL изображения должен быть валидным", new[] { nameof(ImageUrl) }));

        return results;
    }
}