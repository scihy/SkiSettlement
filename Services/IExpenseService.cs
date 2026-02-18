using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public interface IExpenseService
{
    Task<IReadOnlyList<Expense>> GetByTripIdAsync(int tripId, CancellationToken cancellationToken = default);
    Task<Expense> CreateAsync(Expense expense, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Expense expense, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
