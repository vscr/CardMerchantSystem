using Fee.Domain.Entities;
using Fee.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fee.Infrastructure.Configurations;

public class MerchantTariffConfiguration : IEntityTypeConfiguration<MerchantTariff>
{
    public void Configure(EntityTypeBuilder<MerchantTariff> builder)
    {
        builder.ToTable("MerchantTariffs");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.MerchantId).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(500);

        // Decimal properties
        builder.Property(x => x.SpecialRate).HasPrecision(10, 4);

        // Smart Enum: FeeType
        builder.Property(x => x.FeeType)
            .HasConversion(
                v => v.Id,
                v => FeeType.FromId<FeeType>(v)!)
            .HasColumnName("FeeTypeId")
            .IsRequired();

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.MerchantId);
        builder.HasIndex(x => x.TariffId);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => new { x.MerchantId, x.FeeType, x.IsActive });
    }
}