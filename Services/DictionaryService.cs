using Microsoft.EntityFrameworkCore;
using SkiSettlement.Data;
using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public class DictionaryService : IDictionaryService
{
    private readonly AppDbContext _context;

    public DictionaryService(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<DictionaryItem>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.DictionaryItems.Include(d => d.Category).OrderBy(d => d.Type).ThenBy(d => d.SortOrder).ThenBy(d => d.Description).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<DictionaryItem>> GetByTypeAsync(DictionaryItemType type, CancellationToken cancellationToken = default)
        => await _context.DictionaryItems.Include(d => d.Category).Where(d => d.Type == type).OrderBy(d => d.SortOrder).ThenBy(d => d.Description).ToListAsync(cancellationToken);

    public async Task<DictionaryItem> CreateAsync(DictionaryItem item, CancellationToken cancellationToken = default)
    {
        _context.DictionaryItems.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task<bool> UpdateAsync(DictionaryItem item, CancellationToken cancellationToken = default)
    {
        var existing = await _context.DictionaryItems.FindAsync([item.Id], cancellationToken);
        if (existing is null) return false;
        existing.Type = item.Type;
        existing.Description = item.Description;
        existing.DefaultAmount = item.DefaultAmount;
        existing.SortOrder = item.SortOrder;
        existing.CategoryId = item.CategoryId;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _context.DictionaryItems.FindAsync([id], cancellationToken);
        if (item is null) return false;
        _context.DictionaryItems.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
