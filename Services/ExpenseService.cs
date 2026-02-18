using Microsoft.EntityFrameworkCore;
using SkiSettlement.Data;
using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public class ExpenseService : IExpenseService
{
    private readonly AppDbContext _context;

    public ExpenseService(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<Expense>> GetByTripIdAsync(int tripId, CancellationToken cancellationToken = default)
        => await _context.Expenses.Include(e => e.Category).Where(e => e.TripId == tripId).OrderByDescending(e => e.CreatedAt).ToListAsync(cancellationToken);

    public async Task<Expense> CreateAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        expense.CreatedAt = DateTime.UtcNow;
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync(cancellationToken);
        return expense;
    }

    public async Task<bool> UpdateAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Expenses.FindAsync([expense.Id], cancellationToken);
        if (existing is null) return false;
        existing.Description = expense.Description;
        existing.Amount = expense.Amount;
        existing.CategoryId = expense.CategoryId;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var expense = await _context.Expenses.FindAsync([id], cancellationToken);
        if (expense is null) return false;
        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
