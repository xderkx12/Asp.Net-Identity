using Identity.Application.Abstractions.Persistence;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(IdentityDbContext dbContext) : BaseEntityRepository<User>(dbContext), IUserRepository
{
    public Task<bool> ExistsByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(x => x.Login == login, cancellationToken);
    }

    public Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(x => x.Login == login, cancellationToken);
    }

    public Task<User?> GetByLoginWithRolesAsync(string login, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Login == login, cancellationToken);
    }

    public Task<User?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<User?> GetByPasswordResetTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(x => x.PasswordResetTokenHash == tokenHash, cancellationToken);
    }

    public async Task<IReadOnlyCollection<string>> GetRoleNamesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbContext.UserRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Include(x => x.Role)
            .Select(x => x.Role.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<User>> GetByRoleNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await DbContext.UserRoles
            .AsNoTracking()
            .Where(x => x.Role.Name == roleName)
            .Select(x => x.User)
            .ToListAsync(cancellationToken);
    }
}
