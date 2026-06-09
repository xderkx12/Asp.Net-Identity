namespace Identity.Api.Contracts.Users;

public sealed record BlockUserRequest(DateTime? LockedUntilUtc, string? Reason);
