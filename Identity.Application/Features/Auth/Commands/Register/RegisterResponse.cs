namespace Identity.Application.Features.Auth.Commands.Register;

public sealed record RegisterResponse(Guid UserId, string AccessToken);
