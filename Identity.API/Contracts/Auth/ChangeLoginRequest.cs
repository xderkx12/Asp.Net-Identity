namespace Identity.Api.Contracts.Auth;

public sealed record ChangeLoginRequest(string CurrentPassword, string NewLogin);
