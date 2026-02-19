using Microsoft.EntityFrameworkCore;
using SkiSettlement.Data;
using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public class ExpenseService : IExpenseService
{
    private readonly AppDbContext _context;
    private readonly IAuditLogService _auditLog;

    public ExpenseService(AppDbContext context, IAuditLogService auditLog)
    {
        _context = context;
        _auditLog = auditLog;
    }

    public async Task<IReadOnlyList<Expense>> GetByTripIdAsync(int tripId, CancellationToken cancellationToken = default)
        => await _context.Expenses.Include(e => e.Category).Where(e => e.TripId == tripId).OrderByDescending(e => e.CreatedAt).ToListAsync(cancellationToken);

    public async Task<Expense> CreateAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        expense.CreatedAt = DateTime.UtcNow;
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync(cancellationToken);
        var tripPrefix = await GetTripNamePrefixAsync(expense.TripId, cancellationToken);
        await _auditLog.LogAsync("Dodano", "Koszt", expense.Id, $"{tripPrefix}{expense.Description} — {expense.Amount:N2} zł", cancellationToken);
        return expense;
    }

    public async Task<bool> UpdateAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Expenses.FindAsync([expense.Id], cancellationToken);
        if (existing is null) return false;
        existing.Description = expense.Description;
        existing.Amount = expense.Amount;
        existing.CategoryId = expense.CategoryId;
        existing.IsPaid = expense.IsPaid;
        await _context.SaveChangesAsync(cancellationToken);
        var tripPrefix = await GetTripNamePrefixAsync(expense.TripId, cancellationToken);
        await _auditLog.LogAsync("Zmieniono", "Koszt", expense.Id, $"{tripPrefix}{expense.Description} — {expense.Amount:N2} zł", cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var expense = await _context.Expenses.FindAsync([id], cancellationToken);
        if (expense is null) return false;
        var tripPrefix = await GetTripNamePrefixAsync(expense.TripId, cancellationToken);
        var desc = $"{tripPrefix}{expense.Description} — {expense.Amount:N2} zł";
        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditLog.LogAsync("Usunięto", "Koszt", id, desc, cancellationToken);
        return true;
    }

    private async Task<string> GetTripNamePrefixAsync(int tripId, CancellationToken cancellationToken)
    {
        var name = await _context.Trips.AsNoTracking().Where(t => t.Id == tripId).Select(t => t.Name).FirstOrDefaultAsync(cancellationToken);
        return string.IsNullOrEmpty(name) ? "" : $"Wyjazd: {name} · ";
    }
}
