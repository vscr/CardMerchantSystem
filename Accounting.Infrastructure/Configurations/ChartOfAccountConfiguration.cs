using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.Infrastructure.Configurations;

public class ChartOfAccountConfiguration : IEntityTypeConfiguration<ChartOfAccount>
{
    public void Configure(EntityTypeBuilder<ChartOfAccount> builder)
    {
        builder.ToTable("ChartOfAccounts");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.AccountCode).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.AccountCode).IsUnique();

        builder.Property(x => x.AccountName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.CurrencyCode).HasMaxLength(3);
        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.UpdatedBy).HasMaxLength(50);

        // Smart Enum: AccountType
        builder.Property(x => x.AccountType)
            .HasConversion(
                v => v.Id,
                v => AccountType.FromId<AccountType>(v)!)
            .HasColumnName("AccountTypeId")
            .IsRequired();

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.AccountType);
        builder.HasIndex(x => x.ParentAccountId);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.Level);
    }
}