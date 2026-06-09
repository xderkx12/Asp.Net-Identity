namespace Identity.Application.Features.Users.Commands.ActivateUserByLogin;

public sealed record ActivateUserByLoginResponse(string Login, bool IsActive);
