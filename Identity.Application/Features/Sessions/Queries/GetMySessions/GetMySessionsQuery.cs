using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Sessions.Queries.GetMySessions;

public sealed record GetMySessionsQuery(Guid UserId) : IQuery<GetMySessionsResponse>;
