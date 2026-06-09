namespace Identity.Application.Features.Users.Queries.GetUserRolesByLogin;

public sealed record GetUserRolesByLoginResponse(string Login, IReadOnlyCollection<string> Roles);
