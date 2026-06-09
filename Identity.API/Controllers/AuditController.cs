using Identity.Application.Features.Audit.Queries.GetAuditLog;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[Route("api/audit-log")]
[Authorize(Roles = "admin")]
public sealed class AuditController(IMediator mediator) : BaseController(mediator)
{
    [HttpGet]
    public Task<IActionResult> Get(
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        [FromQuery] string? action,
        [FromQuery] string? actorLogin,
        [FromQuery] bool? success,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        return HandleRequestAsync(new GetAuditLogQuery(fromUtc, toUtc, action, actorLogin, success, page, pageSize));
    }
}
