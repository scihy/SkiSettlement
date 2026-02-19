namespace SkiSettlement.Data.Models;

public class Income
{
    public int Id { get; set; }
    public int TripId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    /// <summary>Czy przychód został odebrany / opłacony (Płatność: Tak/Nie).</summary>
    public bool IsPaid { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Trip Trip { get; set; } = null!;
}
