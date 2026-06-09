namespace Identity.Application.Common.Audit;

public sealed record AuditEvent
{
    public required string Action { get; init; }
    public bool Success { get; init; } = true;
    public Guid? ActorUserId { get; init; }
    public string? ActorLogin { get; init; }
    public string? TargetType { get; init; }
    public string? TargetId { get; init; }
    public string? TargetName { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
    public string? Details { get; init; }
}
