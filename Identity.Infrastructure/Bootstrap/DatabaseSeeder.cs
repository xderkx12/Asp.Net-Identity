using Identity.Application.Abstractions.Security;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Bootstrap;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<BootstrapOptions>>().Value;
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeder");

        var adminRole = await EnsureAdminRoleAsync(dbContext, options.AdminRoleName, cancellationToken);
        await EnsureAdminUserAsync(dbContext, passwordHasher, options, adminRole, logger, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task<Role> EnsureAdminRoleAsync(
        IdentityDbContext dbContext,
        string adminRoleName,
        CancellationToken cancellationToken)
    {
        var role = await dbContext.Roles.FirstOrDefaultAsync(x => x.Name == adminRoleName, cancellationToken);
        if (role is not null)
        {
            return role;
        }

        role = new Role(Guid.NewGuid(), adminRoleName);
        await dbContext.Roles.AddAsync(role, cancellationToken);
        return role;
    }

    private static async Task EnsureAdminUserAsync(
        IdentityDbContext dbContext,
        IPasswordHasher passwordHasher,
        BootstrapOptions options,
        Role adminRole,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(options.AdminLogin) || string.IsNullOrWhiteSpace(options.AdminPassword))
        {
            logger.LogWarning(
                "Bootstrap admin credentials are not configured. Skipping admin user seeding. " +
                "Set Bootstrap:AdminLogin and Bootstrap:AdminPassword to enable.");
            return;
        }

        var existingAdmin = await dbContext.Users
            .Include(x => x.UserRoles)
            .FirstOrDefaultAsync(x => x.Login == options.AdminLogin, cancellationToken);

        if (existingAdmin is null)
        {
            existingAdmin = new User(Guid.NewGuid(), options.AdminLogin, passwordHasher.Hash(options.AdminPassword));
            await dbContext.Users.AddAsync(existingAdmin, cancellationToken);
            logger.LogInformation("Bootstrap admin user '{Login}' created.", options.AdminLogin);
        }

        if (existingAdmin.UserRoles.All(x => x.RoleId != adminRole.Id))
        {
            existingAdmin.AssignRole(adminRole.Id);
            logger.LogInformation("Bootstrap admin role assigned to '{Login}'.", options.AdminLogin);
        }
    }
}
