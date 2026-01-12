using Fee.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fee.Infrastructure.Configurations;

public class CommissionBreakdownConfiguration : IEntityTypeConfiguration<CommissionBreakdown>
{
    public void Configure(EntityTypeBuilder<CommissionBreakdown> builder)
    {
        builder.ToTable("CommissionBreakdowns");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.MerchantId).HasMaxLength(20).IsRequired();
        builder.Property(x => x.MCC).HasMaxLength(4);

        // Decimal properties
        builder.Property(x => x.TransactionAmount).HasPrecision(18, 2);
        builder.Property(x => x.TotalCommission).HasPrecision(18, 2);
        builder.Property(x => x.BankShare).HasPrecision(18, 2);
        builder.Property(x => x.InterchangeFee).HasPrecision(18, 2);
        builder.Property(x => x.BKMFee).HasPrecision(18, 2);
        builder.Property(x => x.MerchantDiscount).HasPrecision(18, 2);
        builder.Property(x => x.CommissionRate).HasPrecision(10, 4);
        builder.Property(x => x.BankShareRate).HasPrecision(10, 4);
        builder.Property(x => x.InterchangeRate).HasPrecision(10, 4);
        builder.Property(x => x.BKMRate).HasPrecision(10, 4);
        builder.Property(x => x.MerchantNetAmount).HasPrecision(18, 2);

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.TransactionId).IsUnique();
        builder.HasIndex(x => x.MerchantId);
        builder.HasIndex(x => x.TransactionDate);
    }
}