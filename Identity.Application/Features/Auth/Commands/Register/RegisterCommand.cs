using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(string Login, string Password) : ICommand<RegisterResponse>;
