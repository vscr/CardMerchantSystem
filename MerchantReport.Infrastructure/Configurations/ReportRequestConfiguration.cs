using MerchantReport.Domain.Entities;
using MerchantReport.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MerchantReport.Infrastructure.Configurations;

public class ReportRequestConfiguration : IEntityTypeConfiguration<ReportRequest>
{
    public void Configure(EntityTypeBuilder<ReportRequest> builder)
    {
        builder.ToTable("ReportRequests");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.RequestNumber).HasMaxLength(30).IsRequired();
        builder.HasIndex(x => x.RequestNumber).IsUnique();

        builder.Property(x => x.MerchantId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.MerchantName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.FileName).HasMaxLength(200);
        builder.Property(x => x.FilePath).HasMaxLength(500);
        builder.Property(x => x.DeliveryDetails).HasMaxLength(500);
        builder.Property(x => x.ErrorMessage).HasMaxLength(1000);
        builder.Property(x => x.RequestedBy).HasMaxLength(50);
        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.UpdatedBy).HasMaxLength(50);

        // Decimal properties
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.TotalCommission).HasPrecision(18, 2);
        builder.Property(x => x.NetAmount).HasPrecision(18, 2);

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

        // Smart Enum: ReportStatus
        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => ReportStatus.FromId<ReportStatus>(v)!)
            .HasColumnName("StatusId")
            .IsRequired();

        // Smart Enum: DeliveryMethod
        builder.Property(x => x.DeliveryMethod)
            .HasConversion(
                v => v.Id,
                v => DeliveryMethod.FromId<DeliveryMethod>(v)!)
            .HasColumnName("DeliveryMethodId")
            .IsRequired();

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.MerchantId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.PeriodStart);
        builder.HasIndex(x => x.PeriodEnd);
        builder.HasIndex(x => x.CreatedAt);
    }
}