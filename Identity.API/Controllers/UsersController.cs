using Identity.Api.Contracts.Users;
using Identity.Application.Features.Users.Commands.ActivateUserByLogin;
using Identity.Application.Features.Users.Commands.AssignRoleByLogin;
using Identity.Application.Features.Users.Commands.BlockUserByLogin;
using Identity.Application.Features.Users.Commands.DeactivateUserByLogin;
using Identity.Application.Features.Users.Commands.RevokeRoleByLogin;
using Identity.Application.Features.Users.Commands.UnblockUserByLogin;
using Identity.Application.Features.Users.Queries.GetUserRolesByLogin;
using Identity.Application.Features.Users.Queries.GetUsersByRole;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public sealed class UsersController(IMediator mediator) : BaseController(mediator)
{
    [HttpPost("{login}/roles")]
    public Task<IActionResult> AssignRole([FromRoute] string login, [FromBody] AssignRoleByLoginRequest request)
    {
        return HandleRequestAsync(new AssignRoleByLoginCommand(login, request.RoleName));
    }

    [HttpDelete("{login}/roles/{roleName}")]
    public Task<IActionResult> RevokeRole([FromRoute] string login, [FromRoute] string roleName)
    {
        return HandleRequestAsync(new RevokeRoleByLoginCommand(login, roleName));
    }

    [HttpGet("{login}/roles")]
    public Task<IActionResult> GetRoles([FromRoute] string login)
    {
        return HandleRequestAsync(new GetUserRolesByLoginQuery(login));
    }

    [HttpGet("by-role/{roleName}")]
    public Task<IActionResult> GetByRole([FromRoute] string roleName)
    {
        return HandleRequestAsync(new GetUsersByRoleQuery(roleName));
    }

    [HttpPost("{login}/block")]
    public Task<IActionResult> Block(
        [FromRoute] string login,
        [FromBody] BlockUserRequest? request = null)
    {
        var body = request ?? new BlockUserRequest(null, null);
        return HandleRequestAsync(new BlockUserByLoginCommand(login, body.LockedUntilUtc, body.Reason));
    }

    [HttpPost("{login}/unblock")]
    public Task<IActionResult> Unblock([FromRoute] string login)
    {
        return HandleRequestAsync(new UnblockUserByLoginCommand(login));
    }

    [HttpPost("{login}/activate")]
    public Task<IActionResult> Activate([FromRoute] string login)
    {
        return HandleRequestAsync(new ActivateUserByLoginCommand(login));
    }

    [HttpPost("{login}/deactivate")]
    public Task<IActionResult> Deactivate([FromRoute] string login)
    {
        return HandleRequestAsync(new DeactivateUserByLoginCommand(login));
    }
}
