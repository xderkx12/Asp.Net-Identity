using Identity.Application.Abstractions.Persistence;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public sealed class AuditLogRepository(IdentityDbContext dbContext) : IAuditLogRepository
{
    public Task AddAsync(AuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        return dbContext.AuditLog.AddAsync(entry, cancellationToken).AsTask();
    }

    public async Task<(IReadOnlyCollection<AuditLogEntry> Items, int TotalCount)> QueryAsync(
        AuditLogQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize switch
        {
            < 1 => 50,
            > 500 => 500,
            _ => query.PageSize
        };

        var queryable = dbContext.AuditLog.AsNoTracking();

        if (query.FromUtc is not null)
        {
            queryable = queryable.Where(x => x.TimestampUtc >= query.FromUtc);
        }

        if (query.ToUtc is not null)
        {
            queryable = queryable.Where(x => x.TimestampUtc <= query.ToUtc);
        }

        if (!string.IsNullOrWhiteSpace(query.Action))
        {
            queryable = queryable.Where(x => x.Action == query.Action);
        }

        if (!string.IsNullOrWhiteSpace(query.ActorLogin))
        {
            queryable = queryable.Where(x => x.ActorLogin == query.ActorLogin);
        }

        if (query.Success is not null)
        {
            queryable = queryable.Where(x => x.Success == query.Success);
        }

        var totalCount = await queryable.CountAsync(cancellationToken);

        var items = await queryable
            .OrderByDescending(x => x.TimestampUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
