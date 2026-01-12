using Fee.Domain.Entities;
using Fee.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fee.Infrastructure.Configurations;

public class FeeAccrualConfiguration : IEntityTypeConfiguration<FeeAccrual>
{
    public void Configure(EntityTypeBuilder<FeeAccrual> builder)
    {
        builder.ToTable("FeeAccruals");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.AccrualNumber).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.AccrualNumber).IsUnique();

        builder.Property(x => x.MerchantId).HasMaxLength(20);
        builder.Property(x => x.CardNumber).HasMaxLength(25);
        builder.Property(x => x.TerminalId).HasMaxLength(20);
        builder.Property(x => x.AccrualPeriodStart).HasMaxLength(6).IsRequired();
        builder.Property(x => x.AccrualPeriodEnd).HasMaxLength(6).IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.UpdatedBy).HasMaxLength(50);

        // Decimal properties
        builder.Property(x => x.GrossAmount).HasPrecision(18, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.NetAmount).HasPrecision(18, 2);
        builder.Property(x => x.PaidAmount).HasPrecision(18, 2);
        builder.Property(x => x.RemainingAmount).HasPrecision(18, 2);

        // Smart Enum: FeeType
        builder.Property(x => x.FeeType)
            .HasConversion(
                v => v.Id,
                v => FeeType.FromId<FeeType>(v)!)
            .HasColumnName("FeeTypeId")
            .IsRequired();

        // Smart Enum: Status
        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => AccrualStatus.FromId<AccrualStatus>(v)!)
            .HasColumnName("StatusId")
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
        builder.HasIndex(x => x.MerchantId);
        builder.HasIndex(x => x.CardNumber);
        builder.HasIndex(x => x.TerminalId);
        builder.HasIndex(x => x.DueDate);
        builder.HasIndex(x => x.Status);
    }
}