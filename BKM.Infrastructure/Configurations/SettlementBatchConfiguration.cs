using BKM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BKM.Infrastructure.Configurations;

public class SettlementBatchConfiguration : IEntityTypeConfiguration<SettlementBatch>
{
    public void Configure(EntityTypeBuilder<SettlementBatch> builder)
    {
        builder.ToTable("SettlementBatches");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.SettlementDate).HasMaxLength(8).IsRequired();
        builder.Property(x => x.BatchNumber).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.BatchNumber).IsUnique();

        // Decimal properties
        builder.Property(x => x.TotalTransactionAmount).HasPrecision(18, 2);
        builder.Property(x => x.TotalFeeAmount).HasPrecision(18, 2);
        builder.Property(x => x.TotalNetAmount).HasPrecision(18, 2);

        // Relationships
        builder.HasMany(x => x.BankSummaries)
            .WithOne()
            .HasForeignKey(x => x.SettlementBatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(SettlementBatch.BankSummaries))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.SettlementDate);
        builder.HasIndex(x => x.IsCompleted);
    }
}