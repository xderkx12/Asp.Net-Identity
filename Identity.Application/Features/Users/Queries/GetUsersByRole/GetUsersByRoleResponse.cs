namespace Identity.Application.Features.Users.Queries.GetUsersByRole;

public sealed record GetUsersByRoleResponse(string RoleName, IReadOnlyCollection<GetUsersByRoleItem> Users);
