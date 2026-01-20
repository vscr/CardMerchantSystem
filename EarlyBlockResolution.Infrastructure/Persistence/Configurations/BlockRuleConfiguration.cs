using CardMerchantSystem.Shared.Kernel;
using EarlyBlockResolution.Domain.Entities;
using EarlyBlockResolution.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyBlockResolution.Infrastructure.Persistence.Configurations;

public class BlockRuleConfiguration : IEntityTypeConfiguration<BlockRule>
{
    public void Configure(EntityTypeBuilder<BlockRule> builder)
    {
        builder.ToTable("BlockRules");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.AmountThreshold)
            .HasPrecision(18, 2);

        builder.Property(x => x.TriggerReason)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<BlockReason>(v)!)
            .HasColumnName("TriggerReasonId");

        builder.Property(x => x.Severity)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<AlertSeverity>(v)!)
            .HasColumnName("SeverityId");

        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.UpdatedBy).HasMaxLength(100);
    }
}