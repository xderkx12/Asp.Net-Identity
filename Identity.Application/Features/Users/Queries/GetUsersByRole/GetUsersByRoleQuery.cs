using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Users.Queries.GetUsersByRole;

public sealed record GetUsersByRoleQuery(string RoleName) : IQuery<GetUsersByRoleResponse>;
