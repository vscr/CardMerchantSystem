using Campaign.Domain.Entities;
using Campaign.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Campaign.Infrastructure.Configurations;

public class CampaignConfiguration : IEntityTypeConfiguration<CampaignAggregate>
{
    public void Configure(EntityTypeBuilder<CampaignAggregate> builder)
    {
        builder.ToTable("Campaigns");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.CampaignCode).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.CampaignCode).IsUnique();

        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.ApprovedBy).HasMaxLength(50);
        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.UpdatedBy).HasMaxLength(50);

        // Decimal properties
        builder.Property(x => x.DiscountValue).HasPrecision(18, 2);
        builder.Property(x => x.MaxDiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.MinTransactionAmount).HasPrecision(18, 2);
        builder.Property(x => x.TotalBudget).HasPrecision(18, 2);
        builder.Property(x => x.UsedBudget).HasPrecision(18, 2);

        // Smart Enum: Status
        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => CampaignStatus.FromId<CampaignStatus>(v)!)
            .HasColumnName("StatusId")
            .IsRequired();

        // Smart Enum: CampaignType
        builder.Property(x => x.CampaignType)
            .HasConversion(
                v => v.Id,
                v => CampaignType.FromId<CampaignType>(v)!)
            .HasColumnName("CampaignTypeId")
            .IsRequired();

        // Smart Enum: DiscountType
        builder.Property(x => x.DiscountType)
            .HasConversion(
                v => v.Id,
                v => DiscountType.FromId<DiscountType>(v)!)
            .HasColumnName("DiscountTypeId")
            .IsRequired();

        // Smart Enum: TargetAudience
        builder.Property(x => x.TargetAudience)
            .HasConversion(
                v => v.Id,
                v => TargetAudience.FromId<TargetAudience>(v)!)
            .HasColumnName("TargetAudienceId")
            .IsRequired();

        // AllowedMerchantIds - JSON olarak kaydet
        builder.Property(x => x.AllowedMerchantIds)
            .HasConversion(
                v => v == null ? null : string.Join(",", v),
                v => string.IsNullOrEmpty(v) ? null : v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList())
            .HasMaxLength(2000);

        // Relationships
        builder.HasMany(x => x.Rules)
            .WithOne()
            .HasForeignKey(x => x.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Usages)
            .WithOne()
            .HasForeignKey(x => x.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(CampaignAggregate.Rules))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindNavigation(nameof(CampaignAggregate.Usages))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.StartDate);
        builder.HasIndex(x => x.EndDate);
        builder.HasIndex(x => x.CreatedAt);
    }
}