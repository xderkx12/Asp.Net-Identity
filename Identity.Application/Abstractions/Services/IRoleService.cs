using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Services;

public interface IRoleService
{
    Task<Role> CreateRoleAsync(string name, CancellationToken cancellationToken = default);
    Task DeleteRoleAsync(string name, CancellationToken cancellationToken = default);
    Task<Role> RenameRoleAsync(string currentName, string newName, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Role>> GetAllRolesAsync(CancellationToken cancellationToken = default);

    Task AssignToUserAsync(string login, string roleName, CancellationToken cancellationToken = default);
    Task RevokeFromUserAsync(string login, string roleName, CancellationToken cancellationToken = default);
}
