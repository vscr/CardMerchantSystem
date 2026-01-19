using MerchantSettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MerchantSettlement.Infrastructure.Persistence.Configurations;

public class MerchantReconciliationMismatchConfiguration : IEntityTypeConfiguration<MerchantReconciliationMismatch>
{
    public void Configure(EntityTypeBuilder<MerchantReconciliationMismatch> builder)
    {
        builder.ToTable("MerchantReconciliationMismatches");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TransactionId)
            .HasMaxLength(50);

        builder.Property(x => x.TransactionNumber)
            .HasMaxLength(50);

        builder.Property(x => x.MismatchType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.SystemAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.ReportedAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.AmountDifference)
            .HasPrecision(18, 2);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.ResolutionNotes)
            .HasMaxLength(500);
    }
}