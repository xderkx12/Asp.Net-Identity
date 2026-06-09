namespace Identity.Application.Abstractions.Persistence;

public sealed record AuditLogQuery(
    DateTime? FromUtc = null,
    DateTime? ToUtc = null,
    string? Action = null,
    string? ActorLogin = null,
    bool? Success = null,
    int Page = 1,
    int PageSize = 50);
