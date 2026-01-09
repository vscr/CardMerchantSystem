using Campaign.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Campaign.Infrastructure.Configurations;

public class CampaignUsageConfiguration : IEntityTypeConfiguration<CampaignUsage>
{
    public void Configure(EntityTypeBuilder<CampaignUsage> builder)
    {
        builder.ToTable("CampaignUsages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CardNumberMasked).HasMaxLength(25).IsRequired();
        builder.Property(x => x.MerchantCode).HasMaxLength(15).IsRequired();

        builder.Property(x => x.OriginalAmount).HasPrecision(18, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.FinalAmount).HasPrecision(18, 2);

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.CampaignId);
        builder.HasIndex(x => x.TransactionId);
        builder.HasIndex(x => x.CardNumberMasked);
        builder.HasIndex(x => x.MerchantId);
        builder.HasIndex(x => x.UsedAt);
    }
}