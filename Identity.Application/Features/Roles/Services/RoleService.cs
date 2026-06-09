using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Services;
using Identity.Application.Common.Exceptions;
using Identity.Domain.Entities;

namespace Identity.Application.Features.Roles.Services;

internal sealed class RoleService(
    IRoleRepository roleRepository,
    IUserRepository userRepository) : IRoleService
{
    public async Task<Role> CreateRoleAsync(string name, CancellationToken ct)
    {
        var roleName = name.Trim();

        var exists = await roleRepository.ExistsByNameAsync(roleName, ct);
        if (exists)
            throw new ConflictException("Role with this name already exists.");

        var role = new Role(Guid.NewGuid(), roleName);
        await roleRepository.AddAsync(role, ct);
        await roleRepository.SaveChangesAsync(ct);

        return role;
    }

    public async Task DeleteRoleAsync(string name, CancellationToken ct)
    {
        var roleName = name.Trim();

        var role = await roleRepository.GetByNameAsync(roleName, ct);
        if (role is null)
            throw new NotFoundException("Role not found.");

        var hasUsers = await roleRepository.HasAssignedUsersAsync(role.Id, ct);
        if (hasUsers)
            throw new ConflictException("Role is assigned to one or more users. Revoke it from all users before deleting.");

        roleRepository.Remove(role);
        await roleRepository.SaveChangesAsync(ct);
    }

    public async Task<Role> RenameRoleAsync(string currentName, string newName, CancellationToken ct)
    {
        var trimmedCurrent = currentName.Trim();
        var trimmedNew = newName.Trim();

        var role = await roleRepository.GetByNameAsync(trimmedCurrent, ct);
        if (role is null)
            throw new NotFoundException("Role not found.");

        if (string.Equals(trimmedCurrent, trimmedNew, StringComparison.Ordinal))
            return role;

        var conflict = await roleRepository.ExistsByNameAsync(trimmedNew, ct);
        if (conflict)
            throw new ConflictException("Role with the new name already exists.");

        role.Rename(trimmedNew);
        await roleRepository.SaveChangesAsync(ct);

        return role;
    }

    public async Task<IReadOnlyCollection<Role>> GetAllRolesAsync(CancellationToken ct)
    {
        return await roleRepository.GetAllAsync(ct);
    }

    public async Task AssignToUserAsync(string login, string roleName, CancellationToken ct)
    {
        var user = await userRepository.GetByLoginWithRolesAsync(login, ct)
                   ?? throw new NotFoundException("User not found.");

        var role = await roleRepository.GetByNameAsync(roleName, ct)
                   ?? throw new NotFoundException("Role not found.");

        if (!user.AssignRole(role.Id))
            throw new ConflictException("Role is already assigned to user.");

        userRepository.Update(user);
        await roleRepository.SaveChangesAsync(ct);
    }

    public async Task RevokeFromUserAsync(string login, string roleName, CancellationToken ct)
    {
        var user = await userRepository.GetByLoginWithRolesAsync(login, ct)
                   ?? throw new NotFoundException("User not found.");

        var role = await roleRepository.GetByNameAsync(roleName, ct)
                   ?? throw new NotFoundException("Role not found.");

        if (!user.RevokeRole(role.Id))
            throw new ConflictException("Role is not assigned to user.");

        userRepository.Update(user);
        await roleRepository.SaveChangesAsync(ct);
    }
}
