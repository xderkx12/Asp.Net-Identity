using Ardalis.Result;
using Identity.Application.Abstractions.Services;
using MediatR;

namespace Identity.Application.Features.Users.Commands.AssignRoleByLogin;

public sealed class AssignRoleByLoginCommandHandler(
    IRoleService roleService)
    : IRequestHandler<AssignRoleByLoginCommand, Result<AssignRoleByLoginResponse>>
{
    public async Task<Result<AssignRoleByLoginResponse>> Handle(AssignRoleByLoginCommand request, CancellationToken cancellationToken)
    {
        await roleService.AssignToUserAsync(request.Login, request.RoleName, cancellationToken);
        return Result.Success(new AssignRoleByLoginResponse(request.Login, request.RoleName, true));
    }
}
