using BKM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BKM.Infrastructure.Configurations;

public class BankSettlementSummaryConfiguration : IEntityTypeConfiguration<BankSettlementSummary>
{
    public void Configure(EntityTypeBuilder<BankSettlementSummary> builder)
    {
        builder.ToTable("BankSettlementSummaries");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.BankCode).HasMaxLength(10).IsRequired();

        // Decimal properties
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.TotalFee).HasPrecision(18, 2);
        builder.Property(x => x.NetAmount).HasPrecision(18, 2);

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.SettlementBatchId);
        builder.HasIndex(x => x.BankCode);
    }
}