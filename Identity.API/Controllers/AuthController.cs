using Identity.Api.Contracts.Auth;
using Identity.Api.Extensions;
using Identity.Application.Features.Auth.Commands.ChangeLogin;
using Identity.Application.Features.Auth.Commands.ChangePassword;
using Identity.Application.Features.Auth.Commands.ForgotPassword;
using Identity.Application.Features.Auth.Commands.Login;
using Identity.Application.Features.Auth.Commands.Logout;
using Identity.Application.Features.Auth.Commands.Refresh;
using Identity.Application.Features.Auth.Commands.Register;
using Identity.Application.Features.Auth.Commands.ResetPassword;
using Identity.Application.Features.Users.Commands.DeactivateMyAccount;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[Route("api/[controller]")]
public sealed class AuthController(IMediator mediator) : BaseController(mediator)
{
    private const string RefreshTokenCookieName = "refreshToken";

    [HttpPost("register")]
    public Task<IActionResult> Register([FromBody] RegisterCommand request)
    {
        return HandleRequestAsync(request);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand request)
    {
        var result = await SendRequestAsync(request);
        if (result.IsSuccess && result.Value is not null)
        {
            AppendRefreshTokenCookie(result.Value.RefreshToken);
        }

        return result.ToActionResult();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        if (!Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken) || string.IsNullOrWhiteSpace(refreshToken))
        {
            return UnauthorizedResponse("Refresh token cookie is missing.");
        }

        var result = await SendRequestAsync(new RefreshCommand(refreshToken));
        if (result.IsSuccess && result.Value is not null)
        {
            AppendRefreshTokenCookie(result.Value.RefreshToken);
        }

        return result.ToActionResult();
    }

    [HttpPost("logout")]
    [Authorize]
    public Task<IActionResult> Logout()
    {
        return RevokeCurrentRefreshTokenCookie();
    }

    [HttpPost("revoke")]
    [Authorize]
    public Task<IActionResult> Revoke()
    {
        return RevokeCurrentRefreshTokenCookie();
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return UnauthorizedResponse("User id claim is missing or invalid.");
        }

        return await HandleRequestAsync(new ChangePasswordCommand(currentUserId.Value, request.CurrentPassword, request.NewPassword));
    }

    [HttpPost("change-login")]
    [Authorize]
    public async Task<IActionResult> ChangeLogin([FromBody] ChangeLoginRequest request)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return UnauthorizedResponse("User id claim is missing or invalid.");
        }

        return await HandleRequestAsync(new ChangeLoginCommand(currentUserId.Value, request.CurrentPassword, request.NewLogin));
    }

    [HttpPost("forgot-password")]
    public Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        return HandleRequestAsync(new ForgotPasswordCommand(request.Login));
    }

    [HttpPost("reset-password")]
    public Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        return HandleRequestAsync(new ResetPasswordCommand(request.ResetToken, request.NewPassword));
    }

    [HttpDelete("me")]
    [Authorize]
    public async Task<IActionResult> DeactivateMyAccount([FromBody] DeactivateMyAccountRequest request)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return UnauthorizedResponse("User id claim is missing or invalid.");
        }

        var result = await SendRequestAsync(new DeactivateMyAccountCommand(currentUserId.Value, request.CurrentPassword));
        if (result.IsSuccess)
        {
            DeleteRefreshTokenCookie();
        }

        return result.ToActionResult();
    }

    private async Task<IActionResult> RevokeCurrentRefreshTokenCookie()
    {
        if (!Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken) || string.IsNullOrWhiteSpace(refreshToken))
        {
            return UnauthorizedResponse("Refresh token cookie is missing.");
        }

        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return UnauthorizedResponse("User id claim is missing or invalid.");
        }

        DeleteRefreshTokenCookie();
        return await HandleRequestAsync(new LogoutCommand(refreshToken, currentUserId.Value));
    }

    private void AppendRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append(
            RefreshTokenCookieName,
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
    }

    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete(
            RefreshTokenCookieName,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
    }
}
