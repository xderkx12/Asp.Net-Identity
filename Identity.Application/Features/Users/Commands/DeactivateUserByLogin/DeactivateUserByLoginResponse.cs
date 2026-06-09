namespace Identity.Application.Features.Users.Commands.DeactivateUserByLogin;

public sealed record DeactivateUserByLoginResponse(string Login, bool IsActive, int RevokedSessions);
