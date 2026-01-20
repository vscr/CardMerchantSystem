using CardMerchantSystem.Shared.Kernel;
using Courier.Domain.Entities;
using Courier.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courier.Infrastructure.Persistence.Configurations;

public class DeliveryAttemptConfiguration : IEntityTypeConfiguration<DeliveryAttempt>
{
    public void Configure(EntityTypeBuilder<DeliveryAttempt> builder)
    {
        builder.ToTable("DeliveryAttempts");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.ShipmentId);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.CourierName)
            .HasMaxLength(100);

        builder.Property(x => x.CourierPhone)
            .HasMaxLength(20);

        builder.Property(x => x.FailureReason)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<DeliveryFailureReason>(v)!)
            .HasColumnName("FailureReasonId");
    }
}