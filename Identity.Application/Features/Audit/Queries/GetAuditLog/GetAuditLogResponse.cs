namespace Identity.Application.Features.Audit.Queries.GetAuditLog;

public sealed record GetAuditLogResponse(
    int Page,
    int PageSize,
    int TotalCount,
    IReadOnlyCollection<GetAuditLogItem> Items);
