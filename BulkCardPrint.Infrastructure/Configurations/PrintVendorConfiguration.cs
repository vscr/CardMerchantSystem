using BulkCardPrint.Domain.Entities;
using BulkCardPrint.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BulkCardPrint.Infrastructure.Persistence.Configurations;

public class PrintVendorConfiguration : IEntityTypeConfiguration<PrintVendor>
{
    public void Configure(EntityTypeBuilder<PrintVendor> builder)
    {
        builder.ToTable("PrintVendors");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ContactPerson)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ContactEmail)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ContactPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.ApiEndpoint)
            .HasMaxLength(500);

        builder.Property(x => x.FtpHost)
            .HasMaxLength(200);

        builder.Property(x => x.FtpUsername)
            .HasMaxLength(100);

        builder.Property(x => x.FtpPath)
            .HasMaxLength(500);

        builder.Property(x => x.PreferredFileFormat)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<FileFormat>(v)!)
            .HasColumnName("FileFormatId");

        // Audit fields
        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.UpdatedBy).HasMaxLength(100);
    }
}