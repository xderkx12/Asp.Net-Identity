using Ardalis.Result;
using Identity.Application.Abstractions.Services;
using MediatR;

namespace Identity.Application.Features.Users.Commands.RevokeRoleByLogin;

public sealed class RevokeRoleByLoginCommandHandler(
    IRoleService roleService)
    : IRequestHandler<RevokeRoleByLoginCommand, Result<RevokeRoleByLoginResponse>>
{
    public async Task<Result<RevokeRoleByLoginResponse>> Handle(RevokeRoleByLoginCommand request, CancellationToken cancellationToken)
    {
        await roleService.RevokeFromUserAsync(request.Login, request.RoleName, cancellationToken);
        return Result.Success(new RevokeRoleByLoginResponse(request.Login, request.RoleName, true));
    }
}
