using Microsoft.EntityFrameworkCore;
using SkiSettlement.Data;
using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public class TripService : ITripService
{
    private readonly AppDbContext _context;
    private readonly IAuditLogService _auditLog;

    public TripService(AppDbContext context, IAuditLogService auditLog)
    {
        _context = context;
        _auditLog = auditLog;
    }

    public async Task<IReadOnlyList<Trip>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Trips
            .OrderByDescending(t => t.DateFrom)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<(Trip Trip, decimal Balance)>> GetAllWithBalanceAsync(CancellationToken cancellationToken = default)
    {
        var trips = await _context.Trips
            .OrderByDescending(t => t.DateFrom)
            .ToListAsync(cancellationToken);
        var tripIds = trips.Select(t => t.Id).ToList();
        var incomeByTrip = await _context.Incomes
            .Where(i => tripIds.Contains(i.TripId))
            .GroupBy(i => i.TripId)
            .Select(g => new { TripId = g.Key, Total = g.Sum(i => (double)i.Amount) })
            .ToListAsync(cancellationToken);
        var expenseByTrip = await _context.Expenses
            .Where(e => tripIds.Contains(e.TripId))
            .GroupBy(e => e.TripId)
            .Select(g => new { TripId = g.Key, Total = g.Sum(e => (double)e.Amount) })
            .ToListAsync(cancellationToken);
        var incomeDict = incomeByTrip.ToDictionary(x => x.TripId, x => x.Total);
        var expenseDict = expenseByTrip.ToDictionary(x => x.TripId, x => x.Total);
        return trips
            .Select(t => (Trip: t, Balance: (decimal)(incomeDict.GetValueOrDefault(t.Id) - expenseDict.GetValueOrDefault(t.Id))))
            .ToList();
    }

    public async Task<Trip?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Trips.FindAsync([id], cancellationToken);
    }

    public async Task<Trip> CreateAsync(Trip trip, CancellationToken cancellationToken = default)
    {
        trip.CreatedAt = DateTime.UtcNow;
        _context.Trips.Add(trip);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditLog.LogAsync("Dodano", "Wyjazd", trip.Id, $"Wyjazd: {trip.Name}", cancellationToken);
        return trip;
    }

    public async Task<bool> UpdateAsync(Trip trip, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Trips.FindAsync([trip.Id], cancellationToken);
        if (existing is null)
            return false;

        existing.Name = trip.Name;
        existing.Destination = trip.Destination;
        existing.Description = trip.Description;
        existing.DateFrom = trip.DateFrom;
        existing.DateTo = trip.DateTo;
        await _context.SaveChangesAsync(cancellationToken);
        await _auditLog.LogAsync("Zmieniono", "Wyjazd", trip.Id, $"Wyjazd: {trip.Name}", cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var trip = await _context.Trips.FindAsync([id], cancellationToken);
        if (trip is null)
            return false;
        var desc = $"Wyjazd: {trip.Name}";
        _context.Trips.Remove(trip);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditLog.LogAsync("Usunięto", "Wyjazd", id, desc, cancellationToken);
        return true;
    }
}
