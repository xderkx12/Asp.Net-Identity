using Identity.Api.Contracts.Roles;
using Identity.Application.Features.Roles.Commands.CreateRole;
using Identity.Application.Features.Roles.Commands.DeleteRole;
using Identity.Application.Features.Roles.Commands.RenameRole;
using Identity.Application.Features.Roles.Queries.GetAllRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public sealed class RolesController(IMediator mediator) : BaseController(mediator)
{
    [HttpGet]
    public Task<IActionResult> GetAll()
    {
        return HandleRequestAsync(new GetAllRolesQuery());
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public Task<IActionResult> Create([FromBody] CreateRoleRequest request)
    {
        return HandleRequestAsync(new CreateRoleCommand(request.Name));
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{name}")]
    public Task<IActionResult> Delete([FromRoute] string name)
    {
        return HandleRequestAsync(new DeleteRoleCommand(name));
    }

    [Authorize(Roles = "admin")]
    [HttpPatch("{currentName}")]
    public Task<IActionResult> Rename([FromRoute] string currentName, [FromBody] RenameRoleRequest request)
    {
        return HandleRequestAsync(new RenameRoleCommand(currentName, request.NewName));
    }
}
