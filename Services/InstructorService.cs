using Microsoft.EntityFrameworkCore;
using SkiSettlement.Data;
using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public class InstructorService : IInstructorService
{
    private readonly AppDbContext _context;
    private readonly IAuditLogService _auditLog;

    public InstructorService(AppDbContext context, IAuditLogService auditLog)
    {
        _context = context;
        _auditLog = auditLog;
    }

    public async Task<IReadOnlyList<Instructor>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Instructors.Include(i => i.Weeks).OrderBy(i => i.LastName).ThenBy(i => i.FirstName).ToListAsync(cancellationToken);

    public async Task<Instructor?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Instructors.Include(i => i.Weeks).FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<Instructor> CreateAsync(Instructor instructor, CancellationToken cancellationToken = default)
    {
        instructor.CreatedAt = DateTime.UtcNow;
        _context.Instructors.Add(instructor);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditLog.LogAsync("Dodano", "Instruktor", instructor.Id, $"{instructor.FirstName} {instructor.LastName}", cancellationToken);
        return instructor;
    }

    public async Task<bool> UpdateAsync(Instructor instructor, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Instructors.FindAsync([instructor.Id], cancellationToken);
        if (existing is null) return false;
        existing.FirstName = instructor.FirstName;
        existing.LastName = instructor.LastName;
        existing.BaseSalary = instructor.BaseSalary;
        existing.HourlyRate = instructor.HourlyRate;
        await _context.SaveChangesAsync(cancellationToken);
        await _auditLog.LogAsync("Zmieniono", "Instruktor", instructor.Id, $"{instructor.FirstName} {instructor.LastName}", cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var instructor = await _context.Instructors.FindAsync([id], cancellationToken);
        if (instructor is null) return false;
        var desc = $"{instructor.FirstName} {instructor.LastName}";
        _context.Instructors.Remove(instructor);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditLog.LogAsync("Usunięto", "Instruktor", id, desc, cancellationToken);
        return true;
    }
}
