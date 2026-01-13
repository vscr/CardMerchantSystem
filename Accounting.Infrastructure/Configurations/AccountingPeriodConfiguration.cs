using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.Infrastructure.Configurations;

public class AccountingPeriodConfiguration : IEntityTypeConfiguration<AccountingPeriod>
{
    public void Configure(EntityTypeBuilder<AccountingPeriod> builder)
    {
        builder.ToTable("AccountingPeriods");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.PeriodCode).HasMaxLength(6).IsRequired();
        builder.HasIndex(x => x.PeriodCode).IsUnique();

        builder.Property(x => x.PeriodName).HasMaxLength(50).IsRequired();
        builder.Property(x => x.ClosedBy).HasMaxLength(50);
        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.UpdatedBy).HasMaxLength(50);

        // Smart Enum: Status
        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => PeriodStatus.FromId<PeriodStatus>(v)!)
            .HasColumnName("StatusId")
            .IsRequired();

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.Year);
        builder.HasIndex(x => x.Status);
    }
}