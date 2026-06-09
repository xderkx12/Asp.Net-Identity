using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Auth.Commands.Refresh;

public sealed record RefreshCommand(string RefreshToken) : ICommand<RefreshResponse>;
