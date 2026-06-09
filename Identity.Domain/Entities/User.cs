using Identity.Domain.Common;

namespace Identity.Domain.Entities;

public sealed class User : BaseEntity
{
    private User()
    {
    }

    public User(Guid id, string login, string passwordHash)
    {
        Id = id;
        Login = login;
        PasswordHash = passwordHash;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public string Login { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }

    public bool IsActive { get; private set; }
    public DateTime? DeactivatedAtUtc { get; private set; }

    public DateTime? LockedUntilUtc { get; private set; }
    public string? LockReason { get; private set; }

    public string? PasswordResetTokenHash { get; private set; }
    public DateTime? PasswordResetTokenExpiresAtUtc { get; private set; }

    private readonly List<Session> _sessions = [];
    public IReadOnlyCollection<Session> Sessions => _sessions;

    private readonly List<UserRole> _userRoles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;

    public bool AssignRole(Guid roleId)
    {
        var alreadyAssigned = _userRoles.Any(x => x.RoleId == roleId);
        if (alreadyAssigned)
        {
            return false;
        }

        _userRoles.Add(new UserRole(Id, roleId));
        return true;
    }

    public bool RevokeRole(Guid roleId)
    {
        var existing = _userRoles.FirstOrDefault(x => x.RoleId == roleId);
        if (existing is null)
        {
            return false;
        }

        _userRoles.Remove(existing);
        return true;
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        ClearPasswordResetToken();
    }

    public void ChangeLogin(string newLogin)
    {
        Login = newLogin;
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        DeactivatedAtUtc = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;
        DeactivatedAtUtc = null;
    }

    public void Block(DateTime? lockedUntilUtc, string? reason)
    {
        LockedUntilUtc = lockedUntilUtc;
        LockReason = reason;
    }

    public void Unblock()
    {
        LockedUntilUtc = null;
        LockReason = null;
    }

    public bool IsBlockedAt(DateTime utcNow)
    {
        return LockedUntilUtc is not null && LockedUntilUtc > utcNow;
    }

    public bool IsPermanentlyBlocked()
    {
        return LockedUntilUtc == DateTime.MaxValue;
    }

    public void IssuePasswordResetToken(string tokenHash, DateTime expiresAtUtc)
    {
        PasswordResetTokenHash = tokenHash;
        PasswordResetTokenExpiresAtUtc = expiresAtUtc;
    }

    public void ClearPasswordResetToken()
    {
        PasswordResetTokenHash = null;
        PasswordResetTokenExpiresAtUtc = null;
    }

    public bool IsPasswordResetTokenValid(string tokenHash, DateTime utcNow)
    {
        return PasswordResetTokenHash is not null
            && PasswordResetTokenExpiresAtUtc is not null
            && PasswordResetTokenExpiresAtUtc > utcNow
            && string.Equals(PasswordResetTokenHash, tokenHash, StringComparison.Ordinal);
    }
}
