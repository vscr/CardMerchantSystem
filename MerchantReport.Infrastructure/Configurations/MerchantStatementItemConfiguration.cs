using MerchantReport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MerchantReport.Infrastructure.Configurations;

public class MerchantStatementItemConfiguration : IEntityTypeConfiguration<MerchantStatementItem>
{
    public void Configure(EntityTypeBuilder<MerchantStatementItem> builder)
    {
        builder.ToTable("MerchantStatementItems");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.TransactionType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.TransactionId).HasMaxLength(50);
        builder.Property(x => x.ReferenceNumber).HasMaxLength(50);
        builder.Property(x => x.CardNumber).HasMaxLength(20);
        builder.Property(x => x.TerminalId).HasMaxLength(20);
        builder.Property(x => x.Description).HasMaxLength(200);

        // Decimal properties
        builder.Property(x => x.GrossAmount).HasPrecision(18, 2);
        builder.Property(x => x.CommissionRate).HasPrecision(10, 4);
        builder.Property(x => x.CommissionAmount).HasPrecision(18, 2);
        builder.Property(x => x.NetAmount).HasPrecision(18, 2);

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.StatementId);
        builder.HasIndex(x => x.TransactionDate);
        builder.HasIndex(x => x.TransactionType);
    }
}