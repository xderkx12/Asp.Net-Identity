namespace Identity.Application.Features.Auth.Commands.ForgotPassword;

public sealed record ForgotPasswordResponse(string? ResetToken, DateTime? ExpiresAtUtc);
