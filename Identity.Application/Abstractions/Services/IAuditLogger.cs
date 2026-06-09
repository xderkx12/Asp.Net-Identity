using Identity.Application.Common.Audit;

namespace Identity.Application.Abstractions.Services;

public interface IAuditLogger
{
    Task LogAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default);
}
