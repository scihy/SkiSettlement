namespace SkiSettlement.Data.Models;

public class InstructorWeek
{
    public int Id { get; set; }
    public int InstructorId { get; set; }
    /// <summary>Numer tygodnia (np. 1, 2, 3...).</summary>
    public int WeekNumber { get; set; }
    public decimal HoursWorked { get; set; }
    /// <summary>Nazwa dodatku do wypłaty za ten tydzień (np. Premia, Dieta).</summary>
    public string? SupplementName { get; set; }
    /// <summary>Kwota dodatku za ten tydzień (zł).</summary>
    public decimal SupplementAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Instructor Instructor { get; set; } = null!;
}
