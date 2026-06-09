using Ardalis.Result;
using Identity.Application.Abstractions.Persistence;
using MediatR;

namespace Identity.Application.Features.Users.Queries.GetUsersByRole;

public sealed class GetUsersByRoleQueryHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository)
    : IRequestHandler<GetUsersByRoleQuery, Result<GetUsersByRoleResponse>>
{
    public async Task<Result<GetUsersByRoleResponse>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
    {
        var roleExists = await roleRepository.ExistsByNameAsync(request.RoleName, cancellationToken);
        if (!roleExists)
        {
            return Result.NotFound("Role not found.");
        }

        var users = await userRepository.GetByRoleNameAsync(request.RoleName, cancellationToken);
        var items = users
            .Select(x => new GetUsersByRoleItem(x.Id, x.Login, x.IsActive, x.LockedUntilUtc))
            .OrderBy(x => x.Login, StringComparer.Ordinal)
            .ToArray();

        return Result.Success(new GetUsersByRoleResponse(request.RoleName, items));
    }
}
