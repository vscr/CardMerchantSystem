using Dispute.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dispute.Infrastructure.Configurations;

public class DisputeNoteConfiguration : IEntityTypeConfiguration<DisputeNote>
{
    public void Configure(EntityTypeBuilder<DisputeNote> builder)
    {
        builder.ToTable("DisputeNotes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Note).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.CreatedByUser).HasMaxLength(50).IsRequired();

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.DisputeId);
    }
}