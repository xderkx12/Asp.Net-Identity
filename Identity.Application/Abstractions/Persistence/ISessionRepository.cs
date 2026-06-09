using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Persistence;

public interface ISessionRepository : IRepository<Session>
{
    Task<Session?> GetActiveByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken cancellationToken = default);
    Task<Session?> GetActiveByRefreshTokenHashForUserAsync(string refreshTokenHash, Guid userId, CancellationToken cancellationToken = default);
    Task<Session?> GetByIdForUserAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Session>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> RevokeAllActiveForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
