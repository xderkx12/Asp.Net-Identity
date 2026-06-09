using Identity.Application.Abstractions.Persistence;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public sealed class SessionRepository(IdentityDbContext dbContext) : BaseEntityRepository<Session>(dbContext), ISessionRepository
{
    public Task<Session?> GetActiveByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return DbSet
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x => x.RefreshTokenHash == refreshTokenHash &&
                     x.RevokedAtUtc == null &&
                     x.ExpiresAtUtc > now,
                cancellationToken);
    }

    public Task<Session?> GetActiveByRefreshTokenHashForUserAsync(string refreshTokenHash, Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return DbSet.FirstOrDefaultAsync(
            x => x.RefreshTokenHash == refreshTokenHash &&
                 x.UserId == userId &&
                 x.RevokedAtUtc == null &&
                 x.ExpiresAtUtc > now,
            cancellationToken);
    }

    public Task<Session?> GetByIdForUserAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(
            x => x.Id == sessionId && x.UserId == userId,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<Session>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await DbSet
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.RevokedAtUtc == null && x.ExpiresAtUtc > now)
            .OrderByDescending(x => x.LastUsedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> RevokeAllActiveForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var activeSessions = await DbSet
            .Where(x => x.UserId == userId && x.RevokedAtUtc == null && x.ExpiresAtUtc > now)
            .ToListAsync(cancellationToken);

        foreach (var session in activeSessions)
        {
            session.Revoke();
        }

        return activeSessions.Count;
    }
}
