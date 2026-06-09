using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Roles.Commands.RenameRole;

public sealed record RenameRoleCommand(string CurrentName, string NewName) : ICommand<RenameRoleResponse>;
