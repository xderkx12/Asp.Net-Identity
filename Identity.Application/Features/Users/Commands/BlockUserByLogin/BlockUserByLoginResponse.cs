namespace Identity.Application.Features.Users.Commands.BlockUserByLogin;

public sealed record BlockUserByLoginResponse(string Login, DateTime? LockedUntilUtc, string? Reason, int RevokedSessions);
