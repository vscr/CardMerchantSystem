using MerchantSettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MerchantSettlement.Infrastructure.Persistence.Configurations;

public class DailySettlementSummaryConfiguration : IEntityTypeConfiguration<DailySettlementSummary>
{
    public void Configure(EntityTypeBuilder<DailySettlementSummary> builder)
    {
        builder.ToTable("DailySettlementSummaries");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.SettlementDate)
            .IsUnique();

        builder.Property(x => x.TotalSalesAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalRefundAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalChargebackAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalGrossAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalCommission)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalFee)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalNetAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(x => x.FinalizedBy)
            .HasMaxLength(100);

        // Audit fields
        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.UpdatedBy).HasMaxLength(100);
    }
}