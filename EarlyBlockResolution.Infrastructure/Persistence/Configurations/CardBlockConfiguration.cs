using CardMerchantSystem.Shared.Kernel;
using EarlyBlockResolution.Domain.Entities;
using EarlyBlockResolution.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyBlockResolution.Infrastructure.Persistence.Configurations;

public class CardBlockConfiguration : IEntityTypeConfiguration<CardBlock>
{
    public void Configure(EntityTypeBuilder<CardBlock> builder)
    {
        builder.ToTable("CardBlocks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BlockNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.BlockNumber)
            .IsUnique();

        builder.HasIndex(x => x.CardId);
        builder.HasIndex(x => x.FraudAlertId);
        builder.HasIndex(x => x.TransactionId);

        builder.Property(x => x.CardNumberMasked)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.CustomerName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CustomerPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.CustomerEmail)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ResolvedBy)
            .HasMaxLength(100);

        builder.Property(x => x.ResolutionNotes)
            .HasMaxLength(1000);

        builder.Property(x => x.Reason)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<BlockReason>(v)!)
            .HasColumnName("ReasonId");

        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<BlockStatus>(v)!)
            .HasColumnName("StatusId");

        builder.Property(x => x.Severity)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<AlertSeverity>(v)!)
            .HasColumnName("SeverityId");

        builder.HasMany(x => x.Verifications)
            .WithOne()
            .HasForeignKey(x => x.CardBlockId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.UpdatedBy).HasMaxLength(100);
    }
}