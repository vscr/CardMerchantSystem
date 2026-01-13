using Statement.Domain.Entities;
using Statement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Statement.Infrastructure.Configurations;

public class StatementItemConfiguration : IEntityTypeConfiguration<StatementItem>
{
    public void Configure(EntityTypeBuilder<StatementItem> builder)
    {
        builder.ToTable("StatementItems");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.Description).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ReferenceNumber).HasMaxLength(50);
        builder.Property(x => x.MerchantName).HasMaxLength(100);
        builder.Property(x => x.MerchantCategory).HasMaxLength(50);
        builder.Property(x => x.OriginalCurrency).HasMaxLength(3);

        // Decimal properties
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.OriginalAmount).HasPrecision(18, 2);
        builder.Property(x => x.ExchangeRate).HasPrecision(18, 6);

        // Smart Enum: ItemType
        builder.Property(x => x.ItemType)
            .HasConversion(
                v => v.Id,
                v => StatementItemType.FromId<StatementItemType>(v)!)
            .HasColumnName("ItemTypeId")
            .IsRequired();

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.StatementId);
        builder.HasIndex(x => x.TransactionDate);
        builder.HasIndex(x => x.ReferenceNumber);
    }
}