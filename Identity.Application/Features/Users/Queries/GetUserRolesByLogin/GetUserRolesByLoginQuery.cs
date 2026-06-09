using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Users.Queries.GetUserRolesByLogin;

public sealed record GetUserRolesByLoginQuery(string Login) : IQuery<GetUserRolesByLoginResponse>;
