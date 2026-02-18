namespace SkiSettlement.Data.Models;

public class Expense
{
    public int Id { get; set; }
    public int TripId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int? CategoryId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Trip Trip { get; set; } = null!;
    public ExpenseCategory? Category { get; set; }
}
