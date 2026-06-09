namespace Identity.Application.Features.Users.Commands.RevokeRoleByLogin;

public sealed record RevokeRoleByLoginResponse(string Login, string RoleName, bool Revoked);
