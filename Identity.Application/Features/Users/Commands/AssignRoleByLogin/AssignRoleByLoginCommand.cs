using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Users.Commands.AssignRoleByLogin;

public sealed record AssignRoleByLoginCommand(string Login, string RoleName) : ICommand<AssignRoleByLoginResponse>;
