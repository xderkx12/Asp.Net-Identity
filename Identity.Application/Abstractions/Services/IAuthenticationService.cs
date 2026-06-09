using Identity.Application.Features.Auth.Commands.Login;
using Identity.Application.Features.Auth.Commands.Logout;
using Identity.Application.Features.Auth.Commands.Refresh;

namespace Identity.Application.Abstractions.Services;

public interface IAuthenticationService
{
    Task<LoginResponse> LoginAsync(LoginCommand command, CancellationToken cancellationToken = default);
    Task<RefreshResponse> RefreshAsync(RefreshCommand command, CancellationToken cancellationToken = default);
    Task<LogoutResponse> LogoutAsync(LogoutCommand command, CancellationToken cancellationToken = default);
}
