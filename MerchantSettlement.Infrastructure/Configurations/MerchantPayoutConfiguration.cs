using CardMerchantSystem.Shared.Kernel;
using MerchantSettlement.Domain.Entities;
using MerchantSettlement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MerchantSettlement.Infrastructure.Persistence.Configurations;

public class MerchantPayoutConfiguration : IEntityTypeConfiguration<MerchantPayout>
{
    public void Configure(EntityTypeBuilder<MerchantPayout> builder)
    {
        builder.ToTable("MerchantPayouts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PayoutNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.PayoutNumber)
            .IsUnique();

        builder.Property(x => x.MerchantId)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.MerchantId);

        builder.Property(x => x.MerchantName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.BankCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.BankName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Iban)
            .IsRequired()
            .HasMaxLength(34);

        builder.Property(x => x.GrossAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalCommission)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalFee)
            .HasPrecision(18, 2);

        builder.Property(x => x.WithholdingTax)
            .HasPrecision(18, 2);

        builder.Property(x => x.NetAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(x => x.BankReferenceNumber)
            .HasMaxLength(50);

        builder.Property(x => x.TransferDescription)
            .HasMaxLength(200);

        builder.Property(x => x.HoldReason)
            .HasMaxLength(500);

        builder.Property(x => x.FailureReason)
            .HasMaxLength(500);

        // PayoutStatus enum
        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<PayoutStatus>(v)!)
            .HasColumnName("StatusId");

        // SettlementBatchIds JSON olarak sakla
        builder.Property(x => x.SettlementBatchIds)
            .HasConversion(
                v => string.Join(",", v),
                v => v.Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(Guid.Parse)
                    .ToList())
            .HasColumnName("SettlementBatchIds");

        // Audit fields
        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.UpdatedBy).HasMaxLength(100);
    }
}