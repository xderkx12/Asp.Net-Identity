using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Roles.Commands.DeleteRole;

public sealed record DeleteRoleCommand(string Name) : ICommand<DeleteRoleResponse>;
