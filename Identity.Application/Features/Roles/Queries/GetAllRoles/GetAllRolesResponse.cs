namespace Identity.Application.Features.Roles.Queries.GetAllRoles;

public sealed record GetAllRolesResponse(IReadOnlyCollection<GetAllRolesItem> Roles);
