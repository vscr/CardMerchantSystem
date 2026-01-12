using Fee.Domain.Entities;
using Fee.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fee.Infrastructure.Configurations;

public class TariffConfiguration : IEntityTypeConfiguration<Tariff>
{
    public void Configure(EntityTypeBuilder<Tariff> builder)
    {
        builder.ToTable("Tariffs");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.TariffCode).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.TariffCode).IsUnique();

        builder.Property(x => x.TariffName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.UpdatedBy).HasMaxLength(50);

        // Smart Enum: FeeType
        builder.Property(x => x.FeeType)
            .HasConversion(
                v => v.Id,
                v => FeeType.FromId<FeeType>(v)!)
            .HasColumnName("FeeTypeId")
            .IsRequired();

        // Smart Enum: Status
        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => TariffStatus.FromId<TariffStatus>(v)!)
            .HasColumnName("StatusId")
            .IsRequired();

        // Relationships
        builder.HasMany(x => x.Rules)
            .WithOne()
            .HasForeignKey(x => x.TariffId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Tariff.Rules))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.FeeType);
        builder.HasIndex(x => x.IsDefault);
        builder.HasIndex(x => x.EffectiveFrom);
    }
}