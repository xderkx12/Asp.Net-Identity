using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Persistence;

public interface IUserRepository : IRepository<User>
{
    Task<bool> ExistsByLoginAsync(string login, CancellationToken cancellationToken = default);
    Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken = default);
    Task<User?> GetByLoginWithRolesAsync(string login, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByPasswordResetTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<string>> GetRoleNamesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<User>> GetByRoleNameAsync(string roleName, CancellationToken cancellationToken = default);
}
