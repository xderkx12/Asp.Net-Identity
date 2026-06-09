using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : BaseEntityConfiguration<User>
{
    protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.Property(x => x.Login).HasMaxLength(100).IsRequired();
        builder.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.DeactivatedAtUtc);
        builder.Property(x => x.LockedUntilUtc);
        builder.Property(x => x.LockReason).HasMaxLength(500);
        builder.Property(x => x.PasswordResetTokenHash).HasMaxLength(128);
        builder.Property(x => x.PasswordResetTokenExpiresAtUtc);

        builder.HasIndex(x => x.Login).IsUnique();
        builder.HasIndex(x => x.PasswordResetTokenHash);

        builder.HasMany(x => x.UserRoles)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
