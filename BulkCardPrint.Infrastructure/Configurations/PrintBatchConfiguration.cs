using BulkCardPrint.Domain.Entities;
using BulkCardPrint.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BulkCardPrint.Infrastructure.Persistence.Configurations;

public class PrintBatchConfiguration : IEntityTypeConfiguration<PrintBatch>
{
    public void Configure(EntityTypeBuilder<PrintBatch> builder)
    {
        builder.ToTable("PrintBatches");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BatchNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.BatchNumber)
            .IsUnique();

        builder.HasIndex(x => x.PrintVendorId);

        builder.Property(x => x.FileName)
            .HasMaxLength(200);

        builder.Property(x => x.FilePath)
            .HasMaxLength(500);

        builder.Property(x => x.FileChecksum)
            .HasMaxLength(100);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.FailureReason)
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<PrintBatchStatus>(v)!)
            .HasColumnName("StatusId");

        // Vendor ilişkisi
        builder.HasOne(x => x.Vendor)
            .WithMany()
            .HasForeignKey(x => x.PrintVendorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Items ilişkisi
        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.PrintBatchId)
            .OnDelete(DeleteBehavior.Cascade);

        // Audit fields
        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.UpdatedBy).HasMaxLength(100);
    }
}