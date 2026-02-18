using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public interface IInstructorService
{
    Task<IReadOnlyList<Instructor>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Instructor?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Instructor> CreateAsync(Instructor instructor, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Instructor instructor, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
