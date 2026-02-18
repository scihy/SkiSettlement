using Microsoft.EntityFrameworkCore;
using SkiSettlement.Data;
using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<ExpenseCategory>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.ExpenseCategories.OrderBy(c => c.Name).ToListAsync(cancellationToken);

    public async Task<ExpenseCategory> CreateAsync(ExpenseCategory category, CancellationToken cancellationToken = default)
    {
        _context.ExpenseCategories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);
        return category;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var cat = await _context.ExpenseCategories.FindAsync([id], cancellationToken);
        if (cat is null) return false;
        _context.ExpenseCategories.Remove(cat);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
