namespace SkiSettlement.Data.Models;

public class DictionaryItem
{
    public int Id { get; set; }
    public DictionaryItemType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal DefaultAmount { get; set; }
    public int SortOrder { get; set; }
    public int? CategoryId { get; set; }

    public ExpenseCategory? Category { get; set; }
}
