namespace SkiSettlement.Data.Models;

public class InstructorWeek
{
    public int Id { get; set; }
    public int InstructorId { get; set; }
    /// <summary>Numer tygodnia (np. 1, 2, 3...).</summary>
    public int WeekNumber { get; set; }
    public decimal HoursWorked { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Instructor Instructor { get; set; } = null!;
}
