namespace Identity.Application.Features.Users.Queries.GetUsersByRole;

public sealed record GetUsersByRoleItem(Guid UserId, string Login, bool IsActive, DateTime? LockedUntilUtc);
