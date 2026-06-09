using Ardalis.Result;
using Identity.Application.Abstractions.Services;
using MediatR;

namespace Identity.Application.Features.Roles.Queries.GetAllRoles;

public sealed class GetAllRolesQueryHandler(IRoleService roleService)
    : IRequestHandler<GetAllRolesQuery, Result<GetAllRolesResponse>>
{
    public async Task<Result<GetAllRolesResponse>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleService.GetAllRolesAsync(cancellationToken);
        var items = roles
            .Select(x => new GetAllRolesItem(x.Id, x.Name))
            .ToArray();
        return Result.Success(new GetAllRolesResponse(items));
    }
}
