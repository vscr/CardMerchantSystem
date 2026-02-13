using Fraud.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fraud.Infrastructure.Configurations;

public class FraudAlertConfiguration : IEntityTypeConfiguration<FraudAlert>
{
    public void Configure(EntityTypeBuilder<FraudAlert> builder)
    {
        builder.ToTable("FraudAlerts", "fraud");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MaskedCardNo).HasMaxLength(20).IsRequired();
        builder.Property(x => x.MerchantId).HasMaxLength(50);
        builder.Property(x => x.MerchantName).HasMaxLength(200);
        builder.Property(x => x.TransactionAmount).HasPrecision(18, 2);
        builder.Property(x => x.CurrencyCode).HasMaxLength(5);
        builder.Property(x => x.HighestFraudResponseCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>();
        builder.Property(x => x.AssignedTo).HasMaxLength(100);
        builder.Property(x => x.ResolutionNote).HasMaxLength(2000);
        builder.Property(x => x.Decision).HasConversion<int?>();

        builder.HasIndex(x => x.TransactionId).IsUnique();
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.MaskedCardNo);
        builder.HasIndex(x => x.AssignedTo);
        builder.HasIndex(x => x.CreatedAt);
    }
}