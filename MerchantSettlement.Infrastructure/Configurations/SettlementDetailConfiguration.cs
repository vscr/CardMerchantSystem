using MerchantSettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MerchantSettlement.Infrastructure.Persistence.Configurations;

public class SettlementDetailConfiguration : IEntityTypeConfiguration<SettlementDetail>
{
    public void Configure(EntityTypeBuilder<SettlementDetail> builder)
    {
        builder.ToTable("SettlementDetails");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TransactionId)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.TransactionId);

        builder.Property(x => x.TransactionNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.TransactionType)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.CardNumberMasked)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.CardBrand)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.TerminalId)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2);

        builder.Property(x => x.CommissionRate)
            .HasPrecision(5, 4);

        builder.Property(x => x.CommissionAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.FeeAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.NetAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(x => x.OriginalTransactionId)
            .HasMaxLength(50);

        builder.Property(x => x.AuthorizationCode)
            .HasMaxLength(20);

        builder.Property(x => x.ReferenceNumber)
            .HasMaxLength(50);
    }
}