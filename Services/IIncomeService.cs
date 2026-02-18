using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public interface IIncomeService
{
    Task<IReadOnlyList<Income>> GetByTripIdAsync(int tripId, CancellationToken cancellationToken = default);
    Task<Income> CreateAsync(Income income, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Income income, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
