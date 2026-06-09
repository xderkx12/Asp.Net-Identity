using System.Security.Cryptography;
using System.Text;
using Identity.Application.Abstractions.Security;
using Identity.Application.Abstractions.Services;

namespace Identity.Application.Services;

internal sealed class TokenService(IJwtTokenGenerator jwtTokenGenerator) : ITokenService
{
    public string GenerateAccessToken(Guid userId, string login, IReadOnlyCollection<string> roles)
        => jwtTokenGenerator.GenerateAccessToken(userId, login, roles);

    public string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    public string ComputeTokenHash(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
