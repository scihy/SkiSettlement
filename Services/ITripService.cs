using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public interface ITripService
{
    Task<IReadOnlyList<Trip>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<(Trip Trip, decimal Balance)>> GetAllWithBalanceAsync(CancellationToken cancellationToken = default);
    Task<Trip?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Trip> CreateAsync(Trip trip, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Trip trip, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
