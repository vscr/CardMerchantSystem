using Fraud.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fraud.Infrastructure.Configurations;

public class FraudScenarioConfiguration : IEntityTypeConfiguration<FraudScenario>
{
    public void Configure(EntityTypeBuilder<FraudScenario> builder)
    {
        builder.ToTable("FraudScenarios", "fraud");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ScenarioNo).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.CheckMode).HasConversion<int>();
        builder.Property(x => x.FraudResponseCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Score);
        builder.Property(x => x.RunOrder);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.UpdatedBy).HasMaxLength(100);

        builder.HasIndex(x => x.ScenarioNo).IsUnique();
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => new { x.IsActive, x.CheckMode });

        builder.HasOne(x => x.Rule)
            .WithMany()
            .HasForeignKey(x => x.RuleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.FilterRule)
            .WithMany()
            .HasForeignKey(x => x.FilterRuleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}