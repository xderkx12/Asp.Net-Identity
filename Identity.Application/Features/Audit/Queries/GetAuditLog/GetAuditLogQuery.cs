using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Audit.Queries.GetAuditLog;

public sealed record GetAuditLogQuery(
    DateTime? FromUtc,
    DateTime? ToUtc,
    string? Action,
    string? ActorLogin,
    bool? Success,
    int Page = 1,
    int PageSize = 50) : IQuery<GetAuditLogResponse>;
