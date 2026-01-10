using HSM.Domain.Entities;
using HSM.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HSM.Infrastructure.Configurations;

public class HSMDeviceConfiguration : IEntityTypeConfiguration<HSMDevice>
{
    public void Configure(EntityTypeBuilder<HSMDevice> builder)
    {
        builder.ToTable("HSMDevices");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.DeviceName).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => x.DeviceName).IsUnique();

        builder.Property(x => x.IpAddress).HasMaxLength(50).IsRequired();
        builder.Property(x => x.SecondaryIpAddress).HasMaxLength(50);
        builder.Property(x => x.HeaderLength).HasMaxLength(10);
        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.UpdatedBy).HasMaxLength(50);

        // Smart Enum: DeviceType
        builder.Property(x => x.DeviceType)
            .HasConversion(
                v => v.Id,
                v => HSMDeviceType.FromId<HSMDeviceType>(v)!)
            .HasColumnName("DeviceTypeId")
            .IsRequired();

        // Smart Enum: Status
        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => HSMConnectionStatus.FromId<HSMConnectionStatus>(v)!)
            .HasColumnName("StatusId")
            .IsRequired();

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.IsPrimary);
        builder.HasIndex(x => x.IsActive);
    }
}