using CardMerchantSystem.Shared.Kernel;
using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MerchantSettlement.Infrastructure.Persistence.Configurations;

public class MerchantSettlementReconciliationConfiguration : IEntityTypeConfiguration<MerchantReconciliation>
{
    public void Configure(EntityTypeBuilder<MerchantReconciliation> builder)
    {
        builder.ToTable("MerchantSettlementReconciliations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReconciliationNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.ReconciliationNumber)
            .IsUnique();

        builder.HasIndex(x => x.SettlementBatchId);

        builder.Property(x => x.MerchantId)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.MerchantId);

        builder.Property(x => x.MerchantName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.SystemGrossAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.SystemCommission)
            .HasPrecision(18, 2);

        builder.Property(x => x.SystemNetAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.ReportedGrossAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.ReportedCommission)
            .HasPrecision(18, 2);

        builder.Property(x => x.ReportedNetAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.GrossAmountDifference)
            .HasPrecision(18, 2);

        builder.Property(x => x.CommissionDifference)
            .HasPrecision(18, 2);

        builder.Property(x => x.NetAmountDifference)
            .HasPrecision(18, 2);

        builder.Property(x => x.ResolutionNotes)
            .HasMaxLength(1000);

        builder.Property(x => x.ResolvedBy)
            .HasMaxLength(100);

        // ReconciliationStatus enum
        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<ReconciliationStatus>(v)!)
            .HasColumnName("StatusId");

        // Mismatches ilişkisi
        builder.HasMany(x => x.Mismatches)
            .WithOne()
            .HasForeignKey(x => x.ReconciliationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Audit fields
        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.UpdatedBy).HasMaxLength(100);
    }
}