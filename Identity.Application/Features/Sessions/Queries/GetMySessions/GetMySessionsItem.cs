namespace Identity.Application.Features.Sessions.Queries.GetMySessions;

public sealed record GetMySessionsItem(
    Guid SessionId,
    DateTime CreatedAtUtc,
    DateTime LastUsedAtUtc,
    DateTime ExpiresAtUtc,
    string? IpAddress,
    string? UserAgent);
