namespace Identity.Application.Features.Users.Commands.AssignRoleByLogin;

public sealed record AssignRoleByLoginResponse(string Login, string RoleName, bool Assigned);
