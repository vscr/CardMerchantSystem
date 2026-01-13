using Accounting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.Infrastructure.Configurations;

public class JournalEntryLineConfiguration : IEntityTypeConfiguration<JournalEntryLine>
{
    public void Configure(EntityTypeBuilder<JournalEntryLine> builder)
    {
        builder.ToTable("JournalEntryLines");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.AccountCode).HasMaxLength(20).IsRequired();
        builder.Property(x => x.AccountName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(200);

        // Decimal properties
        builder.Property(x => x.DebitAmount).HasPrecision(18, 2);
        builder.Property(x => x.CreditAmount).HasPrecision(18, 2);

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.JournalEntryId);
        builder.HasIndex(x => x.AccountId);
    }
}