using Ardalis.Result;
using Identity.Application.Abstractions.Services;
using MediatR;

namespace Identity.Application.Features.Roles.Commands.DeleteRole;

public sealed class DeleteRoleCommandHandler(IRoleService roleService)
    : IRequestHandler<DeleteRoleCommand, Result<DeleteRoleResponse>>
{
    public async Task<Result<DeleteRoleResponse>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        await roleService.DeleteRoleAsync(request.Name, cancellationToken);
        return Result.Success(new DeleteRoleResponse(request.Name.Trim()));
    }
}
