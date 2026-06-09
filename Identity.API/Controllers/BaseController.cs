using Ardalis.Result;
using System.Security.Claims;
using Identity.Api.Extensions;
using Identity.Api.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
public abstract class BaseController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    protected Task<Result<TResponse>> SendRequestAsync<TResponse>(IRequest<Result<TResponse>> request)
    {
        return _mediator.Send(request);
    }

    protected async Task<IActionResult> HandleRequestAsync<TResponse>(IRequest<Result<TResponse>> request)
    {
        var response = await SendRequestAsync(request);
        return response.ToActionResult();
    }

    protected Guid? GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return null;
        }

        return userId;
    }

    protected string? GetCurrentUserLogin()
    {
        return User.FindFirstValue("unique_name")
               ?? User.FindFirstValue(ClaimTypes.Name)
               ?? User.Identity?.Name;
    }

    protected IActionResult UnauthorizedResponse(params string[] errors)
    {
        var response = new ApiResponse<object>
        {
            Success = false,
            StatusCode = 401,
            Message = "Unauthorized",
            Errors = errors,
            Data = null
        };

        return new ObjectResult(response) { StatusCode = 401 };
    }

    protected IActionResult ForbidResponse(params string[] errors)
    {
        var response = new ApiResponse<object>
        {
            Success = false,
            StatusCode = 403,
            Message = "Forbidden",
            Errors = errors,
            Data = null
        };

        return new ObjectResult(response) { StatusCode = 403 };
    }
}
