using Card.Domain.Entities;
using Card.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Card.Infrastructure.Configurations;

public class CardApplicationStatusHistoryConfiguration : IEntityTypeConfiguration<CardApplicationStatusHistory>
{
    public void Configure(EntityTypeBuilder<CardApplicationStatusHistory> builder)
    {
        builder.ToTable("CardApplicationStatusHistories");

        builder.HasKey(x => x.Id);

        // Smart Enum: Status
        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => CardApplicationStatus.FromId<CardApplicationStatus>(v)!)
            .HasColumnName("StatusId")
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.ChangedBy)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ChangedAt)
            .IsRequired();

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.CardApplicationId);
        builder.HasIndex(x => x.ChangedAt);
    }
}