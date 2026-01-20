using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegulatoryReporting.Domain.Entities;

namespace RegulatoryReporting.Infrastructure.Configurations;

public class ReportSubmissionConfiguration : IEntityTypeConfiguration<ReportSubmission>
{
    public void Configure(EntityTypeBuilder<ReportSubmission> builder)
    {
        builder.ToTable("ReportSubmissions");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.GeneratedReportId);

        builder.Property(x => x.SubmissionMethod)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.SubmissionReference)
            .HasMaxLength(100);

        builder.Property(x => x.SubmittedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ResponseMessage)
            .HasMaxLength(500);

        builder.Property(x => x.ErrorDetails)
            .HasMaxLength(1000);
    }
}