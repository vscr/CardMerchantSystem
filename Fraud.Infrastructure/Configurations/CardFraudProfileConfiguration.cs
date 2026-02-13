using Fraud.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fraud.Infrastructure.Configurations;

public class CardFraudProfileConfiguration : IEntityTypeConfiguration<CardFraudProfile>
{
    public void Configure(EntityTypeBuilder<CardFraudProfile> builder)
    {
        builder.ToTable("CardFraudProfiles", "fraud");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MaskedCardNo).HasMaxLength(20).IsRequired();
        builder.Property(x => x.TotalTransactionAmount).HasPrecision(18, 2);
        builder.Property(x => x.Last1HourTxAmount).HasPrecision(18, 2);
        builder.Property(x => x.Last24HourTxAmount).HasPrecision(18, 2);
        builder.Property(x => x.LastTransactionCountry).HasMaxLength(5);
        builder.Property(x => x.LastMerchantId).HasMaxLength(50);

        builder.HasIndex(x => x.MaskedCardNo).IsUnique();
        builder.HasIndex(x => x.CurrentRiskScore);
    }
}