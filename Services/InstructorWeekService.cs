using Microsoft.EntityFrameworkCore;
using SkiSettlement.Data;
using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public class InstructorWeekService : IInstructorWeekService
{
    private readonly AppDbContext _context;
    private readonly IAuditLogService _auditLog;

    public InstructorWeekService(AppDbContext context, IAuditLogService auditLog)
    {
        _context = context;
        _auditLog = auditLog;
    }

    public async Task<IReadOnlyList<InstructorWeek>> GetByInstructorIdAsync(int instructorId, CancellationToken cancellationToken = default)
        => await _context.InstructorWeeks.Where(w => w.InstructorId == instructorId).OrderBy(w => w.WeekNumber).ToListAsync(cancellationToken);

    public async Task<InstructorWeek> CreateAsync(InstructorWeek week, CancellationToken cancellationToken = default)
    {
        week.CreatedAt = DateTime.UtcNow;
        _context.InstructorWeeks.Add(week);
        await _context.SaveChangesAsync(cancellationToken);
        var instPrefix = await GetInstructorNamePrefixAsync(week.InstructorId, cancellationToken);
        await _auditLog.LogAsync("Dodano", "Tydzień instruktora", week.Id, $"{instPrefix}tydzień {week.WeekNumber}, {week.HoursWorked}h", cancellationToken);
        return week;
    }

    public async Task<bool> UpdateAsync(InstructorWeek week, CancellationToken cancellationToken = default)
    {
        var existing = await _context.InstructorWeeks.FindAsync([week.Id], cancellationToken);
        if (existing is null) return false;
        existing.WeekNumber = week.WeekNumber;
        existing.HoursWorked = week.HoursWorked;
        existing.SupplementName = week.SupplementName;
        existing.SupplementAmount = week.SupplementAmount;
        await _context.SaveChangesAsync(cancellationToken);
        var instPrefix = await GetInstructorNamePrefixAsync(week.InstructorId, cancellationToken);
        await _auditLog.LogAsync("Zmieniono", "Tydzień instruktora", week.Id, $"{instPrefix}tydzień {week.WeekNumber}, {week.HoursWorked}h", cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var week = await _context.InstructorWeeks.FindAsync([id], cancellationToken);
        if (week is null) return false;
        var instPrefix = await GetInstructorNamePrefixAsync(week.InstructorId, cancellationToken);
        var desc = $"{instPrefix}tydzień {week.WeekNumber}";
        _context.InstructorWeeks.Remove(week);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditLog.LogAsync("Usunięto", "Tydzień instruktora", id, desc, cancellationToken);
        return true;
    }

    private async Task<string> GetInstructorNamePrefixAsync(int instructorId, CancellationToken cancellationToken)
    {
        var inst = await _context.Instructors.AsNoTracking().Where(i => i.Id == instructorId)
            .Select(i => new { i.FirstName, i.LastName }).FirstOrDefaultAsync(cancellationToken);
        if (inst is null) return "";
        return $"Instruktor: {inst.FirstName} {inst.LastName} · ";
    }
}
