using CardMerchantSystem.Shared.Kernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegulatoryReporting.Domain.Entities;
using RegulatoryReporting.Domain.Enums;

namespace RegulatoryReporting.Infrastructure.Configurations;

public class ReportDefinitionConfiguration : IEntityTypeConfiguration<ReportDefinition>
{
    public void Configure(EntityTypeBuilder<ReportDefinition> builder)
    {
        builder.ToTable("ReportDefinitions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.TemplateQuery)
            .HasMaxLength(4000);

        builder.Property(x => x.TemplateFilePath)
            .HasMaxLength(500);

        builder.Property(x => x.Authority)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<RegulatoryAuthority>(v)!)
            .HasColumnName("AuthorityId");

        builder.Property(x => x.ReportType)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<ReportType>(v)!)
            .HasColumnName("ReportTypeId");

        builder.Property(x => x.FileFormat)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<ReportFileFormat>(v)!)
            .HasColumnName("FileFormatId");

        builder.Property(x => x.Frequency)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<ReportFrequency>(v)!)
            .HasColumnName("FrequencyId");

        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.UpdatedBy).HasMaxLength(100);
    }
}