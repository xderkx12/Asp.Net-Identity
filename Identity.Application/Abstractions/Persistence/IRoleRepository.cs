using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Persistence;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> HasAssignedUsersAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Role>> GetByUserLoginAsync(string login, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Role>> GetAllAsync(CancellationToken cancellationToken = default);
}
