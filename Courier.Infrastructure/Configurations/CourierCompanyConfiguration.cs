using CardMerchantSystem.Shared.Kernel;
using Courier.Domain.Entities;
using Courier.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courier.Infrastructure.Persistence.Configurations;

public class CourierCompanyConfiguration : IEntityTypeConfiguration<CourierCompany>
{
    public void Configure(EntityTypeBuilder<CourierCompany> builder)
    {
        builder.ToTable("CourierCompanies");

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

        builder.Property(x => x.ContactPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.ContactEmail)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ApiEndpoint)
            .HasMaxLength(500);

        builder.Property(x => x.ApiKey)
            .HasMaxLength(500);

        builder.Property(x => x.FtpHost)
            .HasMaxLength(200);

        builder.Property(x => x.FtpUsername)
            .HasMaxLength(100);

        builder.Property(x => x.FtpPath)
            .HasMaxLength(500);

        builder.Property(x => x.TrackingUrlTemplate)
            .HasMaxLength(500);

        builder.Property(x => x.BasePrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.PricePerKg)
            .HasPrecision(18, 2);

        builder.Property(x => x.CompanyType)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<CourierCompanyType>(v)!)
            .HasColumnName("CompanyTypeId");

        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.UpdatedBy).HasMaxLength(100);
    }
}