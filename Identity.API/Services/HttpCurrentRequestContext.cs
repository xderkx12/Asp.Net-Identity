using System.Security.Claims;
using Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Http;

namespace Identity.Api.Services;

public sealed class HttpCurrentRequestContext : ICurrentRequestContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentRequestContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpContext? Context => _httpContextAccessor.HttpContext;

    public Guid? UserId
    {
        get
        {
            var user = Context?.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? user.FindFirstValue("sub");

            return Guid.TryParse(userIdValue, out var userId) ? userId : null;
        }
    }

    public string? Login
    {
        get
        {
            var user = Context?.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            return user.FindFirstValue("unique_name")
                   ?? user.FindFirstValue(ClaimTypes.Name)
                   ?? user.Identity.Name;
        }
    }

    public string? IpAddress => Context is null
        ? null
        : NormalizeIp(Context.Connection.RemoteIpAddress);

    public string? UserAgent
    {
        get
        {
            var context = Context;
            if (context is null)
            {
                return null;
            }

            if (!context.Request.Headers.TryGetValue("User-Agent", out var values))
            {
                return null;
            }

            var value = values.ToString();
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
    }

    private static string? NormalizeIp(System.Net.IPAddress? address)
    {
        if (address is null)
        {
            return null;
        }

        if (address.IsIPv4MappedToIPv6)
        {
            return address.MapToIPv4().ToString();
        }

        return address.ToString();
    }
}
