using Microsoft.EntityFrameworkCore;
using SkiSettlement.Data;
using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public class InstructorWeekService : IInstructorWeekService
{
    private readonly AppDbContext _context;

    public InstructorWeekService(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<InstructorWeek>> GetByInstructorIdAsync(int instructorId, CancellationToken cancellationToken = default)
        => await _context.InstructorWeeks.Where(w => w.InstructorId == instructorId).OrderBy(w => w.WeekNumber).ToListAsync(cancellationToken);

    public async Task<InstructorWeek> CreateAsync(InstructorWeek week, CancellationToken cancellationToken = default)
    {
        week.CreatedAt = DateTime.UtcNow;
        _context.InstructorWeeks.Add(week);
        await _context.SaveChangesAsync(cancellationToken);
        return week;
    }

    public async Task<bool> UpdateAsync(InstructorWeek week, CancellationToken cancellationToken = default)
    {
        var existing = await _context.InstructorWeeks.FindAsync([week.Id], cancellationToken);
        if (existing is null) return false;
        existing.WeekNumber = week.WeekNumber;
        existing.HoursWorked = week.HoursWorked;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var week = await _context.InstructorWeeks.FindAsync([id], cancellationToken);
        if (week is null) return false;
        _context.InstructorWeeks.Remove(week);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
