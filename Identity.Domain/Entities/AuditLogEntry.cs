using Identity.Domain.Common;

namespace Identity.Domain.Entities;

public sealed class AuditLogEntry : BaseEntity
{
    private AuditLogEntry()
    {
    }

    public AuditLogEntry(
        Guid id,
        DateTime timestampUtc,
        string action,
        bool success,
        Guid? actorUserId,
        string? actorLogin,
        string? targetType,
        string? targetId,
        string? targetName,
        string? ipAddress,
        string? userAgent,
        string? details)
    {
        Id = id;
        TimestampUtc = timestampUtc;
        Action = action;
        Success = success;
        ActorUserId = actorUserId;
        ActorLogin = actorLogin;
        TargetType = targetType;
        TargetId = targetId;
        TargetName = targetName;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Details = details;
    }

    public DateTime TimestampUtc { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public bool Success { get; private set; }
    public Guid? ActorUserId { get; private set; }
    public string? ActorLogin { get; private set; }
    public string? TargetType { get; private set; }
    public string? TargetId { get; private set; }
    public string? TargetName { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? Details { get; private set; }
}
