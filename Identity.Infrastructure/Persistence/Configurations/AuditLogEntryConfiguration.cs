using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public sealed class AuditLogEntryConfiguration : BaseEntityConfiguration<AuditLogEntry>
{
    protected override void ConfigureEntity(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.ToTable("audit_log");

        builder.Property(x => x.TimestampUtc).IsRequired();
        builder.Property(x => x.Action).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Success).IsRequired();
        builder.Property(x => x.ActorUserId);
        builder.Property(x => x.ActorLogin).HasMaxLength(100);
        builder.Property(x => x.TargetType).HasMaxLength(100);
        builder.Property(x => x.TargetId).HasMaxLength(100);
        builder.Property(x => x.TargetName).HasMaxLength(200);
        builder.Property(x => x.IpAddress).HasMaxLength(64);
        builder.Property(x => x.UserAgent).HasMaxLength(512);
        builder.Property(x => x.Details).HasColumnType("text");

        builder.HasIndex(x => x.TimestampUtc);
        builder.HasIndex(x => x.Action);
        builder.HasIndex(x => x.ActorUserId);
        builder.HasIndex(x => x.ActorLogin);
    }
}
