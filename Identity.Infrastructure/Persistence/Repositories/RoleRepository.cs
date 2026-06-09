using Identity.Application.Abstractions.Persistence;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public sealed class RoleRepository(IdentityDbContext dbContext)
    : BaseEntityRepository<Role>(dbContext), IRoleRepository
{
    public Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(x => x.Name == name, cancellationToken);
    }

    public Task<bool> HasAssignedUsersAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return DbContext.UserRoles.AnyAsync(x => x.RoleId == roleId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Role>> GetByUserLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        return await DbContext.UserRoles
            .AsNoTracking()
            .Where(x => x.User.Login == login)
            .Include(x => x.Role)
            .Select(x => x.Role)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}
