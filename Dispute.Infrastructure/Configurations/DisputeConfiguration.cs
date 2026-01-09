using Dispute.Domain.Entities;
using Dispute.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dispute.Infrastructure.Configurations;

public class DisputeConfiguration : IEntityTypeConfiguration<DisputeAggregate>
{
    public void Configure(EntityTypeBuilder<DisputeAggregate> builder)
    {
        builder.ToTable("Disputes");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.DisputeNumber).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.DisputeNumber).IsUnique();

        builder.Property(x => x.TransactionReference).HasMaxLength(50).IsRequired();
        builder.Property(x => x.CustomerTckn).HasMaxLength(11).IsRequired();
        builder.Property(x => x.CustomerName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CustomerPhone).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CustomerEmail).HasMaxLength(100).IsRequired();
        builder.Property(x => x.MerchantCode).HasMaxLength(15).IsRequired();
        builder.Property(x => x.MerchantName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.CustomerStatement).HasMaxLength(2000);
        builder.Property(x => x.MerchantResponse).HasMaxLength(2000);
        builder.Property(x => x.AssignedTo).HasMaxLength(50);
        builder.Property(x => x.Resolution).HasMaxLength(500);
        builder.Property(x => x.ResolvedBy).HasMaxLength(50);
        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.UpdatedBy).HasMaxLength(50);

        // Decimal properties
        builder.Property(x => x.TransactionAmount).HasPrecision(18, 2);
        builder.Property(x => x.DisputedAmount).HasPrecision(18, 2);
        builder.Property(x => x.RefundAmount).HasPrecision(18, 2);

        // Smart Enum: Status
        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => DisputeStatus.FromId<DisputeStatus>(v)!)
            .HasColumnName("StatusId")
            .IsRequired();

        // Smart Enum: Reason
        builder.Property(x => x.Reason)
            .HasConversion(
                v => v.Id,
                v => DisputeReason.FromId<DisputeReason>(v)!)
            .HasColumnName("ReasonId")
            .IsRequired();

        // Smart Enum: Priority
        builder.Property(x => x.Priority)
            .HasConversion(
                v => v.Id,
                v => DisputePriority.FromId<DisputePriority>(v)!)
            .HasColumnName("PriorityId")
            .IsRequired();

        // Relationships
        builder.HasMany(x => x.Documents)
            .WithOne()
            .HasForeignKey(x => x.DisputeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Notes)
            .WithOne()
            .HasForeignKey(x => x.DisputeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(DisputeAggregate.Documents))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindNavigation(nameof(DisputeAggregate.Notes))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.TransactionId);
        builder.HasIndex(x => x.MerchantId);
        builder.HasIndex(x => x.CustomerTckn);
        builder.HasIndex(x => x.DueDate);
        builder.HasIndex(x => x.CreatedAt);
    }
}