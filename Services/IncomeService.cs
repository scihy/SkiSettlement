using Microsoft.EntityFrameworkCore;
using SkiSettlement.Data;
using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public class IncomeService : IIncomeService
{
    private readonly AppDbContext _context;
    private readonly IAuditLogService _auditLog;

    public IncomeService(AppDbContext context, IAuditLogService auditLog)
    {
        _context = context;
        _auditLog = auditLog;
    }

    public async Task<IReadOnlyList<Income>> GetByTripIdAsync(int tripId, CancellationToken cancellationToken = default)
        => await _context.Incomes.Where(i => i.TripId == tripId).OrderByDescending(i => i.CreatedAt).ToListAsync(cancellationToken);

    public async Task<Income> CreateAsync(Income income, CancellationToken cancellationToken = default)
    {
        income.CreatedAt = DateTime.UtcNow;
        _context.Incomes.Add(income);
        await _context.SaveChangesAsync(cancellationToken);
        var tripPrefix = await GetTripNamePrefixAsync(income.TripId, cancellationToken);
        await _auditLog.LogAsync("Dodano", "Przychód", income.Id, $"{tripPrefix}{income.Description} — {income.Amount:N2} zł", cancellationToken);
        return income;
    }

    public async Task<bool> UpdateAsync(Income income, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Incomes.FindAsync([income.Id], cancellationToken);
        if (existing is null) return false;
        existing.Description = income.Description;
        existing.Amount = income.Amount;
        existing.IsPaid = income.IsPaid;
        await _context.SaveChangesAsync(cancellationToken);
        var tripPrefix = await GetTripNamePrefixAsync(income.TripId, cancellationToken);
        await _auditLog.LogAsync("Zmieniono", "Przychód", income.Id, $"{tripPrefix}{income.Description} — {income.Amount:N2} zł", cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var income = await _context.Incomes.FindAsync([id], cancellationToken);
        if (income is null) return false;
        var tripPrefix = await GetTripNamePrefixAsync(income.TripId, cancellationToken);
        var desc = $"{tripPrefix}{income.Description} — {income.Amount:N2} zł";
        _context.Incomes.Remove(income);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditLog.LogAsync("Usunięto", "Przychód", id, desc, cancellationToken);
        return true;
    }

    private async Task<string> GetTripNamePrefixAsync(int tripId, CancellationToken cancellationToken)
    {
        var name = await _context.Trips.AsNoTracking().Where(t => t.Id == tripId).Select(t => t.Name).FirstOrDefaultAsync(cancellationToken);
        return string.IsNullOrEmpty(name) ? "" : $"Wyjazd: {name} · ";
    }
}
