using Statement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Statement.Infrastructure.Configurations;

public class StatementPeriodConfigConfiguration : IEntityTypeConfiguration<StatementPeriodConfig>
{
    public void Configure(EntityTypeBuilder<StatementPeriodConfig> builder)
    {
        builder.ToTable("StatementPeriodConfigs");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.CardNumber).HasMaxLength(16).IsRequired();
        builder.HasIndex(x => x.CardNumber).IsUnique();

        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.UpdatedBy).HasMaxLength(50);

        // Decimal properties
        builder.Property(x => x.InterestRate).HasPrecision(10, 4);
        builder.Property(x => x.CashAdvanceInterestRate).HasPrecision(10, 4);
        builder.Property(x => x.MinimumPaymentRate).HasPrecision(10, 4);
        builder.Property(x => x.MinimumPaymentAmount).HasPrecision(18, 2);

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.StatementDay);
        builder.HasIndex(x => x.IsActive);
    }
}