using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Users.Commands.UnblockUserByLogin;

public sealed record UnblockUserByLoginCommand(string Login) : ICommand<UnblockUserByLoginResponse>;
