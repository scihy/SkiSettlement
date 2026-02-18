namespace SkiSettlement.Data.Models;

public class Trip
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Destination { get; set; }
    public string? Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<Income> Incomes { get; set; } = new List<Income>();
}
