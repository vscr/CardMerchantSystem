using CardMerchantSystem.Shared.Kernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegulatoryReporting.Domain.Entities;
using RegulatoryReporting.Domain.Enums;

namespace RegulatoryReporting.Infrastructure.Configurations;

public class GeneratedReportConfiguration : IEntityTypeConfiguration<GeneratedReport>
{
    public void Configure(EntityTypeBuilder<GeneratedReport> builder)
    {
        builder.ToTable("GeneratedReports");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReportNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.ReportNumber)
            .IsUnique();

        builder.HasIndex(x => x.ReportDefinitionId);

        builder.Property(x => x.FileName)
            .HasMaxLength(200);

        builder.Property(x => x.FilePath)
            .HasMaxLength(500);

        builder.Property(x => x.FileChecksum)
            .HasMaxLength(100);

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.ValidatedBy)
            .HasMaxLength(100);

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(x => x.FileFormat)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<ReportFileFormat>(v)!)
            .HasColumnName("FileFormatId");

        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<ReportStatus>(v)!)
            .HasColumnName("StatusId");

        builder.HasOne(x => x.Definition)
            .WithMany()
            .HasForeignKey(x => x.ReportDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Submissions)
            .WithOne()
            .HasForeignKey(x => x.GeneratedReportId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.UpdatedBy).HasMaxLength(100);
    }
}