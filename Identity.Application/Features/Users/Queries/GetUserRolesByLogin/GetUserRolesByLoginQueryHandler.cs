using Ardalis.Result;
using Identity.Application.Abstractions.Persistence;
using MediatR;

namespace Identity.Application.Features.Users.Queries.GetUserRolesByLogin;

public sealed class GetUserRolesByLoginQueryHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository)
    : IRequestHandler<GetUserRolesByLoginQuery, Result<GetUserRolesByLoginResponse>>
{
    public async Task<Result<GetUserRolesByLoginResponse>> Handle(GetUserRolesByLoginQuery request, CancellationToken cancellationToken)
    {
        var userExists = await userRepository.ExistsByLoginAsync(request.Login, cancellationToken);
        if (!userExists)
        {
            return Result.NotFound(["User not found."]);
        }

        var roles = await roleRepository.GetByUserLoginAsync(request.Login, cancellationToken);
        var roleNames = roles.Select(x => x.Name).Distinct(StringComparer.Ordinal).OrderBy(x => x).ToArray();

        return Result.Success(new GetUserRolesByLoginResponse(request.Login, roleNames));
    }
}
