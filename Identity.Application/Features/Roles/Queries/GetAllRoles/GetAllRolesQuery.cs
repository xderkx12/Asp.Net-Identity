using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Roles.Queries.GetAllRoles;

public sealed record GetAllRolesQuery() : IQuery<GetAllRolesResponse>;
