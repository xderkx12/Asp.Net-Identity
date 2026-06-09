using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Users.Commands.DeactivateUserByLogin;

public sealed record DeactivateUserByLoginCommand(string Login) : ICommand<DeactivateUserByLoginResponse>;
