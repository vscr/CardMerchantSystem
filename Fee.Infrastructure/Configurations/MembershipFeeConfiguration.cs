using Fee.Domain.Entities;
using Fee.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fee.Infrastructure.Configurations;

public class MembershipFeeConfiguration : IEntityTypeConfiguration<MembershipFee>
{
    public void Configure(EntityTypeBuilder<MembershipFee> builder)
    {
        builder.ToTable("MembershipFees");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.FeeName).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => x.FeeName).IsUnique();

        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.UpdatedBy).HasMaxLength(50);

        // Decimal properties
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.LateFeeRate).HasPrecision(10, 4);
        builder.Property(x => x.MinimumTransactionVolume).HasPrecision(18, 2);

        // Smart Enum: FeeType
        builder.Property(x => x.FeeType)
            .HasConversion(
                v => v.Id,
                v => FeeType.FromId<FeeType>(v)!)
            .HasColumnName("FeeTypeId")
            .IsRequired();

        // Smart Enum: Period
        builder.Property(x => x.Period)
            .HasConversion(
                v => v.Id,
                v => AccrualPeriod.FromId<AccrualPeriod>(v)!)
            .HasColumnName("PeriodId")
            .IsRequired();

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.FeeType);
        builder.HasIndex(x => x.IsActive);
    }
}