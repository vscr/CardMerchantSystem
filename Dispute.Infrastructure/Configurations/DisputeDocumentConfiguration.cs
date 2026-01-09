using Dispute.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dispute.Infrastructure.Configurations;

public class DisputeDocumentConfiguration : IEntityTypeConfiguration<DisputeDocument>
{
    public void Configure(EntityTypeBuilder<DisputeDocument> builder)
    {
        builder.ToTable("DisputeDocuments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName).HasMaxLength(255).IsRequired();
        builder.Property(x => x.FileType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.FilePath).HasMaxLength(500).IsRequired();
        builder.Property(x => x.UploadedBy).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.DisputeId);
    }
}