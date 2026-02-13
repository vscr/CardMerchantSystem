using Fraud.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fraud.Infrastructure.Configurations;

public class FraudRuleConditionConfiguration : IEntityTypeConfiguration<FraudRuleCondition>
{
    public void Configure(EntityTypeBuilder<FraudRuleCondition> builder)
    {
        builder.ToTable("FraudRuleConditions", "fraud");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ParameterName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Operator).HasConversion<int>();
        builder.Property(x => x.Value).HasMaxLength(500).IsRequired();
        builder.Property(x => x.SecondValue).HasMaxLength(500);
        builder.Property(x => x.OrderIndex);

        builder.HasIndex(x => x.FraudRuleId);
    }
}