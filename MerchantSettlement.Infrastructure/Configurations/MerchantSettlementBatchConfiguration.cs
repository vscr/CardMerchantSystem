using CardMerchantSystem.Shared.Kernel;
using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MerchantSettlement.Infrastructure.Persistence.Configurations;

public class MerchantSettlementBatchConfiguration : IEntityTypeConfiguration<MerchantSettlementBatch>
{
    public void Configure(EntityTypeBuilder<MerchantSettlementBatch> builder)
    {
        builder.ToTable("MerchantSettlementBatches");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BatchNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.BatchNumber)
            .IsUnique();

        builder.Property(x => x.MerchantId)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.MerchantId);

        builder.Property(x => x.MerchantName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.TotalSalesAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalRefundAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalChargebackAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.GrossAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalCommission)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalFee)
            .HasPrecision(18, 2);

        builder.Property(x => x.NetAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(x => x.ProcessedBy)
            .HasMaxLength(100);

        builder.Property(x => x.FailureReason)
            .HasMaxLength(500);

        // SettlementType enum
        builder.Property(x => x.SettlementType)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<SettlementType>(v)!)
            .HasColumnName("SettlementTypeId");

        // SettlementStatus enum
        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<SettlementStatus>(v)!)
            .HasColumnName("StatusId");

        // Details ilişkisi
        builder.HasMany(x => x.Details)
            .WithOne()
            .HasForeignKey(x => x.SettlementBatchId)
            .OnDelete(DeleteBehavior.Cascade);

        // Audit fields
        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.UpdatedBy).HasMaxLength(100);
    }
}