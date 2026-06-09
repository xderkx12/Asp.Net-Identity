using Identity.Domain.Common;

namespace Identity.Domain.Entities;

public sealed class Session : BaseEntity
{
    private Session()
    {
    }

    public Session(
        Guid id,
        Guid userId,
        string refreshTokenHash,
        DateTime expiresAtUtc,
        string? ipAddress,
        string? userAgent)
    {
        Id = id;
        UserId = userId;
        RefreshTokenHash = refreshTokenHash;
        ExpiresAtUtc = expiresAtUtc;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        CreatedAtUtc = DateTime.UtcNow;
        LastUsedAtUtc = CreatedAtUtc;
    }

    public Guid UserId { get; private set; }
    public string RefreshTokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime LastUsedAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }

    public User User { get; private set; } = null!;

    public bool IsActiveAt(DateTime utcNow)
    {
        return RevokedAtUtc is null && ExpiresAtUtc > utcNow;
    }

    public void Rotate(string refreshTokenHash, DateTime expiresAtUtc, string? ipAddress, string? userAgent)
    {
        RefreshTokenHash = refreshTokenHash;
        ExpiresAtUtc = expiresAtUtc;
        RevokedAtUtc = null;
        LastUsedAtUtc = DateTime.UtcNow;

        if (ipAddress is not null)
        {
            IpAddress = ipAddress;
        }

        if (userAgent is not null)
        {
            UserAgent = userAgent;
        }
    }

    public void Revoke()
    {
        if (RevokedAtUtc is not null)
        {
            return;
        }

        RevokedAtUtc = DateTime.UtcNow;
    }
}
