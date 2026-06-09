using Ardalis.Result;
using Identity.Application.Abstractions.Services;
using MediatR;

namespace Identity.Application.Features.Roles.Commands.CreateRole;

public sealed class CreateRoleCommandHandler(IRoleService roleService)
    : IRequestHandler<CreateRoleCommand, Result<CreateRoleResponse>>
{
    public async Task<Result<CreateRoleResponse>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleService.CreateRoleAsync(request.Name, cancellationToken);
        return Result.Created(new CreateRoleResponse(role.Id, role.Name));
    }
}
