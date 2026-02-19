using System.ComponentModel.DataAnnotations.Schema;

namespace SkiSettlement.Data.Models;

public class Instructor
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public decimal BaseSalary { get; set; }
    public decimal HourlyRate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<InstructorWeek> Weeks { get; set; } = new List<InstructorWeek>();

    [NotMapped]
    public decimal TotalHours => Weeks.Sum(w => w.HoursWorked);

    [NotMapped]
    public decimal AmountFromHours => TotalHours * HourlyRate;

    [NotMapped]
    public decimal TotalSupplementAmount => Weeks.Sum(w => w.SupplementAmount);

    [NotMapped]
    public decimal TotalAmount => BaseSalary + AmountFromHours + TotalSupplementAmount;
}
