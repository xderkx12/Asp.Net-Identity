using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Users.Commands.RevokeRoleByLogin;

public sealed record RevokeRoleByLoginCommand(string Login, string RoleName) : ICommand<RevokeRoleByLoginResponse>;
