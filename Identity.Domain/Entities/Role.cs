using Identity.Domain.Common;

namespace Identity.Domain.Entities;

public sealed class Role : BaseEntity
{
    private Role()
    {
    }

    public Role(Guid id, string name)
    {
        Id = id;
        Name = name;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public string Name { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }

    private readonly List<UserRole> _userRoles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;

    public void Rename(string newName)
    {
        Name = newName;
    }
}
