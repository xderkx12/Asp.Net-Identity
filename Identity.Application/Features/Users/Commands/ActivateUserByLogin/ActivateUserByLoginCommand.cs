using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Users.Commands.ActivateUserByLogin;

public sealed record ActivateUserByLoginCommand(string Login) : ICommand<ActivateUserByLoginResponse>;
