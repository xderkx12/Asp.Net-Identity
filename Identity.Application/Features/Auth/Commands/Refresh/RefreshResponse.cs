using System.Text.Json.Serialization;

namespace Identity.Application.Features.Auth.Commands.Refresh;

public sealed record RefreshResponse(string AccessToken)
{
    [JsonIgnore]
    public string RefreshToken { get; init; } = string.Empty;
}
