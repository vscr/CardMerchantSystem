using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegulatoryReporting.Domain.Entities;

namespace RegulatoryReporting.Infrastructure.Configurations;

public class ReportScheduleConfiguration : IEntityTypeConfiguration<ReportSchedule>
{
    public void Configure(EntityTypeBuilder<ReportSchedule> builder)
    {
        builder.ToTable("ReportSchedules");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.ReportDefinitionId);

        builder.Property(x => x.LastErrorMessage)
            .HasMaxLength(1000);

        builder.HasOne(x => x.Definition)
            .WithMany()
            .HasForeignKey(x => x.ReportDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.UpdatedBy).HasMaxLength(100);
    }
}