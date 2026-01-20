using CardMerchantSystem.Shared.Kernel;
using EarlyBlockResolution.Domain.Entities;
using EarlyBlockResolution.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyBlockResolution.Infrastructure.Persistence.Configurations;

public class FraudAlertConfiguration : IEntityTypeConfiguration<FraudAlert>
{
    public void Configure(EntityTypeBuilder<FraudAlert> builder)
    {
        builder.ToTable("FraudAlerts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AlertNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.AlertNumber)
            .IsUnique();

        builder.HasIndex(x => x.CardId);
        builder.HasIndex(x => x.TransactionId);
        builder.HasIndex(x => x.BlockRuleId);

        builder.Property(x => x.CardNumberMasked)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.MerchantName)
            .HasMaxLength(200);

        builder.Property(x => x.TransactionAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.Reason)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<BlockReason>(v)!)
            .HasColumnName("ReasonId");

        builder.Property(x => x.Severity)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<AlertSeverity>(v)!)
            .HasColumnName("SeverityId");

        builder.HasOne(x => x.TriggerRule)
            .WithMany()
            .HasForeignKey(x => x.BlockRuleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.UpdatedBy).HasMaxLength(100);
    }
}