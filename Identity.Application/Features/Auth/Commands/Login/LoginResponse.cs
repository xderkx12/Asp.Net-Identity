using System.Text.Json.Serialization;

namespace Identity.Application.Features.Auth.Commands.Login;

public sealed record LoginResponse(Guid UserId, string AccessToken)
{
    [JsonIgnore]
    public string RefreshToken { get; init; } = string.Empty;
}
