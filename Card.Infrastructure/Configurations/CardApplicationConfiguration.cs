using Card.Domain.Entities;
using Card.Domain.Enums;
using Card.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Card.Infrastructure.Configurations;

public class CardApplicationConfiguration : IEntityTypeConfiguration<CardApplication>
{
    public void Configure(EntityTypeBuilder<CardApplication> builder)
    {
        builder.ToTable("CardApplications");

        builder.HasKey(x => x.Id);

        // Value Object: TCKN
        builder.OwnsOne(x => x.CustomerTckn, tckn =>
        {
            tckn.Property(t => t.Value)
                .HasColumnName("CustomerTckn")
                .HasMaxLength(11)
                .IsRequired();

            tckn.HasIndex(t => t.Value);
        });

        // Value Object: Address
        builder.OwnsOne(x => x.DeliveryAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("Street").HasMaxLength(200).IsRequired();
            address.Property(a => a.District).HasColumnName("District").HasMaxLength(50).IsRequired();
            address.Property(a => a.City).HasColumnName("City").HasMaxLength(50).IsRequired();
            address.Property(a => a.PostalCode).HasColumnName("PostalCode").HasMaxLength(10).IsRequired();
            address.Property(a => a.Country).HasColumnName("Country").HasMaxLength(50).IsRequired();
            address.Property(a => a.BuildingNo).HasColumnName("BuildingNo").HasMaxLength(20);
            address.Property(a => a.ApartmentNo).HasColumnName("ApartmentNo").HasMaxLength(20);
        });

        // Value Object: DailyLimit
        builder.OwnsOne(x => x.DailyLimit, money =>
        {
            money.Property(m => m.Amount).HasColumnName("DailyLimitAmount").HasPrecision(18, 2).IsRequired();
            money.Property(m => m.Currency).HasColumnName("DailyLimitCurrency").HasMaxLength(3).IsRequired();
        });

        // Value Object: MonthlyLimit
        builder.OwnsOne(x => x.MonthlyLimit, money =>
        {
            money.Property(m => m.Amount).HasColumnName("MonthlyLimitAmount").HasPrecision(18, 2).IsRequired();
            money.Property(m => m.Currency).HasColumnName("MonthlyLimitCurrency").HasMaxLength(3).IsRequired();
        });

        // Smart Enum: CardType
        builder.Property(x => x.CardType)
            .HasConversion(
                v => v.Id,
                v => CardType.FromId<CardType>(v)!)
            .HasColumnName("CardTypeId")
            .IsRequired();

        // Smart Enum: Status
        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => CardApplicationStatus.FromId<CardApplicationStatus>(v)!)
            .HasColumnName("StatusId")
            .IsRequired();

        // Smart Enum: PrintVendor (nullable)
        builder.Property(x => x.PrintVendor)
            .HasConversion(
                v => v == null ? (int?)null : v.Id,
                v => v == null ? null : PrintVendor.FromId<PrintVendor>(v.Value))
            .HasColumnName("PrintVendorId");

        // String properties
        builder.Property(x => x.CustomerName).HasMaxLength(50).IsRequired();
        builder.Property(x => x.CustomerSurname).HasMaxLength(50).IsRequired();
        builder.Property(x => x.PhoneNumber).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CardNumberEncrypted).HasMaxLength(500);
        builder.Property(x => x.CardNumberMasked).HasMaxLength(25);
        builder.Property(x => x.PrintBatchId).HasMaxLength(50);
        builder.Property(x => x.CourierTrackingNumber).HasMaxLength(50);
        builder.Property(x => x.ApprovedBy).HasMaxLength(50);
        builder.Property(x => x.RejectionReason).HasMaxLength(500);
        builder.Property(x => x.RejectedBy).HasMaxLength(50);
        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.UpdatedBy).HasMaxLength(50);

        // Concurrency token
        //builder.Property(x => x.Version).IsConcurrencyToken();

        // Relationship: StatusHistory - private field'a erişim
        builder.HasMany(x => x.StatusHistory)
            .WithOne()
            .HasForeignKey(x => x.CardApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(CardApplication.StatusHistory))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.Email);
        builder.HasIndex(x => x.CreatedAt);
    }
}