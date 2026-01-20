using CardMerchantSystem.Shared.Kernel;
using Courier.Domain.Entities;
using Courier.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courier.Infrastructure.Persistence.Configurations;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("Shipments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ShipmentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.ShipmentNumber)
            .IsUnique();

        builder.Property(x => x.TrackingNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.TrackingNumber)
            .IsUnique();

        builder.Property(x => x.Barcode)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.CourierCompanyId);
        builder.HasIndex(x => x.CardApplicationId);
        builder.HasIndex(x => x.PrintBatchItemId);

        builder.Property(x => x.RecipientName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.RecipientPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.RecipientEmail)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.RecipientTckn)
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(x => x.DeliveryAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.DeliveryDistrict)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.DeliveryCity)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.DeliveryPostalCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.DeliveredToName)
            .HasMaxLength(100);

        builder.Property(x => x.DeliveredToTckn)
            .HasMaxLength(11);

        builder.Property(x => x.SignatureData)
            .HasMaxLength(4000);

        builder.Property(x => x.ShippingCost)
            .HasPrecision(18, 2);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.ShipmentType)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<ShipmentType>(v)!)
            .HasColumnName("ShipmentTypeId");

        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<ShipmentStatus>(v)!)
            .HasColumnName("StatusId");

        builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CourierCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.StatusHistory)
            .WithOne()
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.DeliveryAttempts)
            .WithOne()
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.UpdatedBy).HasMaxLength(100);
    }
}