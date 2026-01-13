using MerchantReport.Domain.Entities;
using MerchantReport.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MerchantReport.Infrastructure.Configurations;

public class MerchantReportConfigConfiguration : IEntityTypeConfiguration<MerchantReportConfig>
{
    public void Configure(EntityTypeBuilder<MerchantReportConfig> builder)
    {
        builder.ToTable("MerchantReportConfigs");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.MerchantId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.MerchantName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.EmailRecipients).HasMaxLength(500);
        builder.Property(x => x.FtpHost).HasMaxLength(200);
        builder.Property(x => x.FtpUsername).HasMaxLength(100);
        builder.Property(x => x.FtpPassword).HasMaxLength(100);
        builder.Property(x => x.FtpPath).HasMaxLength(200);
        builder.Property(x => x.CallbackUrl).HasMaxLength(500);
        builder.Property(x => x.CallbackApiKey).HasMaxLength(200);
        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.UpdatedBy).HasMaxLength(50);

        // Smart Enum: ReportType
        builder.Property(x => x.ReportType)
            .HasConversion(
                v => v.Id,
                v => ReportType.FromId<ReportType>(v)!)
            .HasColumnName("ReportTypeId")
            .IsRequired();

        // Smart Enum: ReportFormat
        builder.Property(x => x.ReportFormat)
            .HasConversion(
                v => v.Id,
                v => ReportFormat.FromId<ReportFormat>(v)!)
            .HasColumnName("ReportFormatId")
            .IsRequired();

        // Smart Enum: DeliveryMethod
        builder.Property(x => x.DeliveryMethod)
            .HasConversion(
                v => v.Id,
                v => DeliveryMethod.FromId<DeliveryMethod>(v)!)
            .HasColumnName("DeliveryMethodId")
            .IsRequired();

        // Smart Enum: Frequency
        builder.Property(x => x.Frequency)
            .HasConversion(
                v => v.Id,
                v => ScheduleFrequency.FromId<ScheduleFrequency>(v)!)
            .HasColumnName("FrequencyId")
            .IsRequired();

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.MerchantId);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.NextRunTime);
    }
}