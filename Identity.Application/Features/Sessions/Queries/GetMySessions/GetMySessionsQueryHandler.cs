using Ardalis.Result;
using Identity.Application.Abstractions.Persistence;
using MediatR;

namespace Identity.Application.Features.Sessions.Queries.GetMySessions;

public sealed class GetMySessionsQueryHandler(ISessionRepository sessionRepository)
    : IRequestHandler<GetMySessionsQuery, Result<GetMySessionsResponse>>
{
    public async Task<Result<GetMySessionsResponse>> Handle(GetMySessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = await sessionRepository.GetActiveByUserIdAsync(request.UserId, cancellationToken);
        var items = sessions
            .Select(x => new GetMySessionsItem(
                x.Id,
                x.CreatedAtUtc,
                x.LastUsedAtUtc,
                x.ExpiresAtUtc,
                x.IpAddress,
                x.UserAgent))
            .ToArray();

        return Result.Success(new GetMySessionsResponse(items));
    }
}
