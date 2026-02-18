using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public interface IDictionaryService
{
    Task<IReadOnlyList<DictionaryItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DictionaryItem>> GetByTypeAsync(DictionaryItemType type, CancellationToken cancellationToken = default);
    Task<DictionaryItem> CreateAsync(DictionaryItem item, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(DictionaryItem item, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
