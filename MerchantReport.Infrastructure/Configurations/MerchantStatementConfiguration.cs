using MerchantReport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MerchantReport.Infrastructure.Configurations;

public class MerchantStatementConfiguration : IEntityTypeConfiguration<MerchantStatement>
{
    public void Configure(EntityTypeBuilder<MerchantStatement> builder)
    {
        builder.ToTable("MerchantStatements");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.StatementNumber).HasMaxLength(30).IsRequired();
        builder.HasIndex(x => x.StatementNumber).IsUnique();

        builder.Property(x => x.MerchantId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.MerchantName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.UpdatedBy).HasMaxLength(50);

        // Decimal properties
        builder.Property(x => x.OpeningBalance).HasPrecision(18, 2);
        builder.Property(x => x.TotalSales).HasPrecision(18, 2);
        builder.Property(x => x.TotalRefunds).HasPrecision(18, 2);
        builder.Property(x => x.TotalCommission).HasPrecision(18, 2);
        builder.Property(x => x.TotalSettlement).HasPrecision(18, 2);
        builder.Property(x => x.ClosingBalance).HasPrecision(18, 2);

        // Relationships
        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.StatementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(MerchantStatement.Items))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.MerchantId);
        builder.HasIndex(x => x.PeriodStart);
        builder.HasIndex(x => x.PeriodEnd);
    }
}