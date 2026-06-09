namespace Identity.Application.Features.Audit.Queries.GetAuditLog;

public sealed record GetAuditLogItem(
    Guid Id,
    DateTime TimestampUtc,
    string Action,
    bool Success,
    Guid? ActorUserId,
    string? ActorLogin,
    string? TargetType,
    string? TargetId,
    string? TargetName,
    string? IpAddress,
    string? UserAgent,
    string? Details);
