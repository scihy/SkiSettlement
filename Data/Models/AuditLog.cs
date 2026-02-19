namespace SkiSettlement.Data.Models;

/// <summary>Wpis logu audytu: kto, kiedy, jaką akcję wykonał na jakim obiekcie.</summary>
public class AuditLog
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>Identyfikator użytkownika (np. IP lub "—" gdy brak).</summary>
    public string UserName { get; set; } = "—";
    /// <summary>Akcja: "Dodano", "Zmieniono", "Usunięto".</summary>
    public string Action { get; set; } = string.Empty;
    /// <summary>Typ encji: "Wyjazd", "Koszt", "Przychód", "Instruktor", "Tydzień instruktora", "Słownik" itd.</summary>
    public string EntityType { get; set; } = string.Empty;
    /// <summary>Id encji (jeśli dotyczy).</summary>
    public int? EntityId { get; set; }
    /// <summary>Opis zmiany (np. "Wyjazd: Zakopane 2025", "Koszt: Hotel 500 zł").</summary>
    public string Description { get; set; } = string.Empty;
}
