namespace backend.Models.Dto.Schedule;

// Уровень "месяц" - показывает дни с доступным временем

// Уровень "день" - показывает конкретные слоты
public class ScheduleDayViewDto
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Description { get; set; }
    public bool IsGroupLesson { get; set; }
    public decimal? IndividualPrice { get; set; }
    public bool IsAvailable { get; set; } // Свободно ли для записи
    public string CoachFullName { get; set; }
    public string GymHallName { get; set; }
}