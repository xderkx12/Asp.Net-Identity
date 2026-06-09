using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Persistence;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLogEntry entry, CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<AuditLogEntry> Items, int TotalCount)> QueryAsync(
        AuditLogQuery query,
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
