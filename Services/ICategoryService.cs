using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public interface ICategoryService
{
    Task<IReadOnlyList<ExpenseCategory>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ExpenseCategory> CreateAsync(ExpenseCategory category, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
