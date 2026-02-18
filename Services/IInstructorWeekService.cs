using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public interface IInstructorWeekService
{
    Task<IReadOnlyList<InstructorWeek>> GetByInstructorIdAsync(int instructorId, CancellationToken cancellationToken = default);
    Task<InstructorWeek> CreateAsync(InstructorWeek week, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(InstructorWeek week, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
