using Merchant.Domain.Entities;
using Merchant.Domain.Enums;
using Merchant.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Merchant.Infrastructure.Configurations;

public class MerchantConfiguration : IEntityTypeConfiguration<MerchantAggregate>
{
    public void Configure(EntityTypeBuilder<MerchantAggregate> builder)
    {
        builder.ToTable("Merchants");

        builder.HasKey(x => x.Id);

        // Value Object: MerchantCode
        builder.OwnsOne(x => x.MerchantCode, mc =>
        {
            mc.Property(m => m.Value)
                .HasColumnName("MerchantCode")
                .HasMaxLength(15)
                .IsRequired();

            mc.HasIndex(m => m.Value).IsUnique();
        });

        // Value Object: TaxNumber
        builder.OwnsOne(x => x.TaxNumber, tn =>
        {
            tn.Property(t => t.Value)
                .HasColumnName("TaxNumber")
                .HasMaxLength(10)
                .IsRequired();

            tn.HasIndex(t => t.Value).IsUnique();
        });

        // Value Object: IBAN
        builder.OwnsOne(x => x.IBAN, iban =>
        {
            iban.Property(i => i.Value)
                .HasColumnName("IBAN")
                .HasMaxLength(26)
                .IsRequired();
        });

        // Smart Enum: MerchantType
        builder.Property(x => x.MerchantType)
            .HasConversion(
                v => v.Id,
                v => MerchantType.FromId<MerchantType>(v)!)
            .HasColumnName("MerchantTypeId")
            .IsRequired();

        // Smart Enum: Status
        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => MerchantStatus.FromId<MerchantStatus>(v)!)
            .HasColumnName("StatusId")
            .IsRequired();

        // String properties
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.TradeName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.TaxOffice).HasMaxLength(100).IsRequired();
        builder.Property(x => x.PhoneNumber).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Address).HasMaxLength(300).IsRequired();
        builder.Property(x => x.City).HasMaxLength(50).IsRequired();
        builder.Property(x => x.District).HasMaxLength(50).IsRequired();
        builder.Property(x => x.ApprovedBy).HasMaxLength(50);
        builder.Property(x => x.RejectionReason).HasMaxLength(500);
        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.UpdatedBy).HasMaxLength(50);

        // Decimal
        builder.Property(x => x.CommissionRate).HasPrecision(5, 2);

        // Relationship: Terminals
        builder.HasMany(x => x.Terminals)
            .WithOne()
            .HasForeignKey(x => x.MerchantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(MerchantAggregate.Terminals))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.Email);
        builder.HasIndex(x => x.CreatedAt);
    }
}