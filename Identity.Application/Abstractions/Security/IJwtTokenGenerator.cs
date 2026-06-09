namespace Identity.Application.Abstractions.Security;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(Guid userId, string login, IReadOnlyCollection<string> roles);
}
