using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(string Login, string Password) : ICommand<LoginResponse>;
