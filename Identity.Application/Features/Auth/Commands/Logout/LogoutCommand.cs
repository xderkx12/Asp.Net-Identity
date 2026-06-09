using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Auth.Commands.Logout;

public sealed record LogoutCommand(string RefreshToken, Guid UserId) : ICommand<LogoutResponse>;
