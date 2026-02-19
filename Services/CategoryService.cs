using Microsoft.EntityFrameworkCore;
using SkiSettlement.Data;
using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;
    private readonly IAuditLogService _auditLog;

    public CategoryService(AppDbContext context, IAuditLogService auditLog)
    {
        _context = context;
        _auditLog = auditLog;
    }

    public async Task<IReadOnlyList<ExpenseCategory>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.ExpenseCategories.OrderBy(c => c.Name).ToListAsync(cancellationToken);

    public async Task<ExpenseCategory> CreateAsync(ExpenseCategory category, CancellationToken cancellationToken = default)
    {
        _context.ExpenseCategories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditLog.LogAsync("Dodano", "Kategoria kosztów", category.Id, category.Name, cancellationToken);
        return category;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var cat = await _context.ExpenseCategories.FindAsync([id], cancellationToken);
        if (cat is null) return false;
        var desc = cat.Name;
        _context.ExpenseCategories.Remove(cat);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditLog.LogAsync("Usunięto", "Kategoria kosztów", id, desc, cancellationToken);
        return true;
    }
}
