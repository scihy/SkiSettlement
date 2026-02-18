using Microsoft.EntityFrameworkCore;
using SkiSettlement.Data;
using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public class IncomeService : IIncomeService
{
    private readonly AppDbContext _context;

    public IncomeService(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<Income>> GetByTripIdAsync(int tripId, CancellationToken cancellationToken = default)
        => await _context.Incomes.Where(i => i.TripId == tripId).OrderByDescending(i => i.CreatedAt).ToListAsync(cancellationToken);

    public async Task<Income> CreateAsync(Income income, CancellationToken cancellationToken = default)
    {
        income.CreatedAt = DateTime.UtcNow;
        _context.Incomes.Add(income);
        await _context.SaveChangesAsync(cancellationToken);
        return income;
    }

    public async Task<bool> UpdateAsync(Income income, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Incomes.FindAsync([income.Id], cancellationToken);
        if (existing is null) return false;
        existing.Description = income.Description;
        existing.Amount = income.Amount;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var income = await _context.Incomes.FindAsync([id], cancellationToken);
        if (income is null) return false;
        _context.Incomes.Remove(income);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
