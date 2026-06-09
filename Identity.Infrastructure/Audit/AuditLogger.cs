using Identity.Application.Abstractions.Services;
using Identity.Application.Common.Audit;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Audit;

public sealed class AuditLogger(IServiceScopeFactory scopeFactory) : IAuditLogger
{
    public async Task LogAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        var entry = new AuditLogEntry(
            Guid.NewGuid(),
            DateTime.UtcNow,
            auditEvent.Action,
            auditEvent.Success,
            auditEvent.ActorUserId,
            auditEvent.ActorLogin,
            auditEvent.TargetType,
            auditEvent.TargetId,
            auditEvent.TargetName,
            auditEvent.IpAddress,
            auditEvent.UserAgent,
            auditEvent.Details);

        dbContext.AuditLog.Add(entry);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
