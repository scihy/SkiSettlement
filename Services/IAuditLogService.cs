using SkiSettlement.Data.Models;

namespace SkiSettlement.Services;

public interface IAuditLogService
{
    Task LogAsync(string action, string entityType, int? entityId, string description, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditLog>> GetRecentAsync(int limit = 500, CancellationToken cancellationToken = default);
}
