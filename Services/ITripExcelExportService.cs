using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public interface ITripExcelExportService
{
    byte[] ExportTripDetails(Trip trip, IReadOnlyList<Expense> expenses, IReadOnlyList<Income> incomes);
}
