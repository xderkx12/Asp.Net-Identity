using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Roles.Commands.CreateRole;

public sealed record CreateRoleCommand(string Name) : ICommand<CreateRoleResponse>;
