using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public sealed class SessionConfiguration : BaseEntityConfiguration<Session>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("sessions");

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.RefreshTokenHash).HasMaxLength(512).IsRequired();
        builder.Property(x => x.ExpiresAtUtc).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.LastUsedAtUtc).IsRequired();
        builder.Property(x => x.RevokedAtUtc);
        builder.Property(x => x.IpAddress).HasMaxLength(64);
        builder.Property(x => x.UserAgent).HasMaxLength(512);
        builder.HasIndex(x => x.RefreshTokenHash);
        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Sessions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
