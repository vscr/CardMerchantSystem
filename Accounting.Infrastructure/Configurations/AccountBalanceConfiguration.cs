using Accounting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.Infrastructure.Configurations;

public class AccountBalanceConfiguration : IEntityTypeConfiguration<AccountBalance>
{
    public void Configure(EntityTypeBuilder<AccountBalance> builder)
    {
        builder.ToTable("AccountBalances");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.AccountCode).HasMaxLength(20).IsRequired();
        builder.Property(x => x.PeriodCode).HasMaxLength(6).IsRequired();

        // Decimal properties
        builder.Property(x => x.OpeningDebit).HasPrecision(18, 2);
        builder.Property(x => x.OpeningCredit).HasPrecision(18, 2);
        builder.Property(x => x.PeriodDebit).HasPrecision(18, 2);
        builder.Property(x => x.PeriodCredit).HasPrecision(18, 2);
        builder.Property(x => x.ClosingDebit).HasPrecision(18, 2);
        builder.Property(x => x.ClosingCredit).HasPrecision(18, 2);

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => new { x.AccountId, x.PeriodCode }).IsUnique();
        builder.HasIndex(x => x.PeriodCode);
    }
}