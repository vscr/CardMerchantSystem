using Fraud.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fraud.Infrastructure.Configurations;

public class FraudRuleConfiguration : IEntityTypeConfiguration<FraudRule>
{
    public void Configure(EntityTypeBuilder<FraudRule> builder)
    {
        builder.ToTable("FraudRules", "fraud");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.RuleType).HasConversion<int>();
        builder.Property(x => x.LogicalOperator).HasConversion<int>();
        builder.Property(x => x.PeriodMinutes);
        builder.Property(x => x.PeriodThreshold).HasPrecision(18, 2);
        builder.Property(x => x.PeriodFunction).HasMaxLength(50);
        builder.Property(x => x.PeriodGroupBy).HasMaxLength(100);
        builder.Property(x => x.SqlScript).HasMaxLength(4000);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.UpdatedBy).HasMaxLength(100);

        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => x.IsActive);

        builder.HasMany(x => x.Conditions)
            .WithOne(x => x.FraudRule)
            .HasForeignKey(x => x.FraudRuleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}