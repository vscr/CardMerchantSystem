using Fee.Domain.Entities;
using Fee.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fee.Infrastructure.Configurations;

public class TariffRuleConfiguration : IEntityTypeConfiguration<TariffRule>
{
    public void Configure(EntityTypeBuilder<TariffRule> builder)
    {
        builder.ToTable("TariffRules");

        builder.HasKey(x => x.Id);

        // Decimal properties
        builder.Property(x => x.Rate).HasPrecision(10, 4);
        builder.Property(x => x.MinimumFee).HasPrecision(18, 2);
        builder.Property(x => x.MaximumFee).HasPrecision(18, 2);
        builder.Property(x => x.VolumeFrom).HasPrecision(18, 2);
        builder.Property(x => x.VolumeTo).HasPrecision(18, 2);

        // String properties
        builder.Property(x => x.MCC).HasMaxLength(4);

        // Smart Enum: CalculationType
        builder.Property(x => x.CalculationType)
            .HasConversion(
                v => v.Id,
                v => CalculationType.FromId<CalculationType>(v)!)
            .HasColumnName("CalculationTypeId")
            .IsRequired();

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.TariffId);
        builder.HasIndex(x => x.MCC);
        builder.HasIndex(x => x.InstallmentCount);
        builder.HasIndex(x => x.IsActive);
    }
}