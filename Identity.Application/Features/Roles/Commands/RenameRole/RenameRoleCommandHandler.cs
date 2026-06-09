using Ardalis.Result;
using Identity.Application.Abstractions.Services;
using MediatR;

namespace Identity.Application.Features.Roles.Commands.RenameRole;

public sealed class RenameRoleCommandHandler(IRoleService roleService)
    : IRequestHandler<RenameRoleCommand, Result<RenameRoleResponse>>
{
    public async Task<Result<RenameRoleResponse>> Handle(RenameRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleService.RenameRoleAsync(request.CurrentName, request.NewName, cancellationToken);
        return Result.Success(new RenameRoleResponse(role.Id, role.Name));
    }
}
