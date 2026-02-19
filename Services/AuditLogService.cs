using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SkiSettlement.Data;
using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public class AuditLogService : IAuditLogService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLogService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogAsync(string action, string entityType, int? entityId, string description, CancellationToken cancellationToken = default)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var userName = "—";
        if (user?.Identity?.IsAuthenticated == true)
        {
            var first = user.FindFirst(ClaimTypes.GivenName)?.Value;
            var last = user.FindFirst(ClaimTypes.Surname)?.Value;
            if (!string.IsNullOrEmpty(first) || !string.IsNullOrEmpty(last))
                userName = $"{first} {last}".Trim();
            if (string.IsNullOrEmpty(userName))
                userName = user.Identity.Name ?? user.FindFirst(ClaimTypes.Email)?.Value ?? "—";
        }
        if (userName == "—")
            userName = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "—";
        var log = new AuditLog
        {
            CreatedAt = DateTime.UtcNow,
            UserName = userName.Length > 200 ? userName[..200] : userName,
            Action = action.Length > 50 ? action[..50] : action,
            EntityType = entityType.Length > 100 ? entityType[..100] : entityType,
            EntityId = entityId,
            Description = description.Length > 1000 ? description[..1000] : description
        };
        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AuditLog>> GetRecentAsync(int limit = 500, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .OrderByDescending(l => l.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
}
