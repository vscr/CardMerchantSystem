using CardMerchantSystem.Shared.Kernel;
using Courier.Domain.Entities;
using Courier.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courier.Infrastructure.Persistence.Configurations;

public class ShipmentStatusHistoryConfiguration : IEntityTypeConfiguration<ShipmentStatusHistory>
{
    public void Configure(EntityTypeBuilder<ShipmentStatusHistory> builder)
    {
        builder.ToTable("ShipmentStatusHistories");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.ShipmentId);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.OperatorUsername)
            .HasMaxLength(100);

        builder.Property(x => x.Location)
            .HasMaxLength(200);

        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<ShipmentStatus>(v)!)
            .HasColumnName("StatusId");
    }
}