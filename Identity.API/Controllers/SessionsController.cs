using Identity.Application.Features.Sessions.Commands.LogoutAllDevices;
using Identity.Application.Features.Sessions.Commands.RevokeSession;
using Identity.Application.Features.Sessions.Queries.GetMySessions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public sealed class SessionsController(IMediator mediator) : BaseController(mediator)
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMy()
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return UnauthorizedResponse("User id claim is missing or invalid.");
        }

        return await HandleRequestAsync(new GetMySessionsQuery(currentUserId.Value));
    }

    [HttpDelete("{sessionId:guid}")]
    public async Task<IActionResult> Revoke([FromRoute] Guid sessionId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return UnauthorizedResponse("User id claim is missing or invalid.");
        }

        return await HandleRequestAsync(new RevokeSessionCommand(currentUserId.Value, sessionId));
    }

    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll()
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return UnauthorizedResponse("User id claim is missing or invalid.");
        }

        return await HandleRequestAsync(new LogoutAllDevicesCommand(currentUserId.Value));
    }
}
