namespace Identity.Application.Features.Sessions.Queries.GetMySessions;

public sealed record GetMySessionsResponse(IReadOnlyCollection<GetMySessionsItem> Sessions);
