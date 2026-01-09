using Campaign.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Campaign.Infrastructure.Configurations;

public class CampaignRuleConfiguration : IEntityTypeConfiguration<CampaignRule>
{
    public void Configure(EntityTypeBuilder<CampaignRule> builder)
    {
        builder.ToTable("CampaignRules");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RuleName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.RuleType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Operator).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(500).IsRequired();

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.CampaignId);
    }
}