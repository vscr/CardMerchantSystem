using BulkCardPrint.Domain.Entities;
using BulkCardPrint.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BulkCardPrint.Infrastructure.Persistence.Configurations;

public class PrintBatchItemConfiguration : IEntityTypeConfiguration<PrintBatchItem>
{
    public void Configure(EntityTypeBuilder<PrintBatchItem> builder)
    {
        builder.ToTable("PrintBatchItems");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.PrintBatchId);

        builder.HasIndex(x => x.CardApplicationId);

        builder.Property(x => x.CustomerName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CustomerSurname)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CustomerTckn)
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(x => x.CardType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.CardNumberEncrypted)
            .HasMaxLength(500);

        builder.Property(x => x.CardNumberMasked)
            .HasMaxLength(20);

        builder.Property(x => x.ExpiryDate)
            .HasMaxLength(4);

        builder.Property(x => x.Cvv)
            .HasMaxLength(10);

        builder.Property(x => x.DeliveryAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.FailureReason)
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<PrintItemStatus>(v)!)
            .HasColumnName("StatusId");
    }
}