using Ardalis.Result;
using Identity.Application.Abstractions.Persistence;
using MediatR;

namespace Identity.Application.Features.Audit.Queries.GetAuditLog;

public sealed class GetAuditLogQueryHandler(IAuditLogRepository auditLogRepository)
    : IRequestHandler<GetAuditLogQuery, Result<GetAuditLogResponse>>
{
    public async Task<Result<GetAuditLogResponse>> Handle(GetAuditLogQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await auditLogRepository.QueryAsync(
            new AuditLogQuery(
                request.FromUtc,
                request.ToUtc,
                request.Action,
                request.ActorLogin,
                request.Success,
                request.Page,
                request.PageSize),
            cancellationToken);

        var mapped = items
            .Select(x => new GetAuditLogItem(
                x.Id,
                x.TimestampUtc,
                x.Action,
                x.Success,
                x.ActorUserId,
                x.ActorLogin,
                x.TargetType,
                x.TargetId,
                x.TargetName,
                x.IpAddress,
                x.UserAgent,
                x.Details))
            .ToArray();

        return Result.Success(new GetAuditLogResponse(request.Page, request.PageSize, totalCount, mapped));
    }
}
