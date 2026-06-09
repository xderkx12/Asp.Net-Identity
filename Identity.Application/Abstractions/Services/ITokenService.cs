namespace Identity.Application.Abstractions.Services;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string login, IReadOnlyCollection<string> roles);
    string GenerateRefreshToken();
    string ComputeTokenHash(string token);
}
