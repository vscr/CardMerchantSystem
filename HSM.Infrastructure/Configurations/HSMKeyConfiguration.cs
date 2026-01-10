using HSM.Domain.Entities;
using HSM.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HSM.Infrastructure.Configurations;

public class HSMKeyConfiguration : IEntityTypeConfiguration<HSMKey>
{
    public void Configure(EntityTypeBuilder<HSMKey> builder)
    {
        builder.ToTable("HSMKeys");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.KeyName).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => x.KeyName).IsUnique();

        builder.Property(x => x.KeyIndex).HasMaxLength(10).IsRequired();
        builder.Property(x => x.EncryptedKeyValue).HasMaxLength(500).IsRequired();
        builder.Property(x => x.KeyCheckValue).HasMaxLength(20);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.UpdatedBy).HasMaxLength(50);

        // Smart Enum: KeyType
        builder.Property(x => x.KeyType)
            .HasConversion(
                v => v.Id,
                v => HSMKeyType.FromId<HSMKeyType>(v)!)
            .HasColumnName("KeyTypeId")
            .IsRequired();

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.KeyIndex);
        builder.HasIndex(x => x.HSMDeviceId);
        builder.HasIndex(x => x.IsActive);
    }
}