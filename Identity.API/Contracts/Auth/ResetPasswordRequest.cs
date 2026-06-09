namespace Identity.Api.Contracts.Auth;

public sealed record ResetPasswordRequest(string ResetToken, string NewPassword);
