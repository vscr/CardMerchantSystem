using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.Infrastructure.Configurations;

public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.ToTable("JournalEntries");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.EntryNumber).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.EntryNumber).IsUnique();

        builder.Property(x => x.PeriodCode).HasMaxLength(6).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500).IsRequired();
        builder.Property(x => x.ReferenceNumber).HasMaxLength(50);
        builder.Property(x => x.ReferenceType).HasMaxLength(50);
        builder.Property(x => x.PostedBy).HasMaxLength(50);
        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.UpdatedBy).HasMaxLength(50);

        // Decimal properties
        builder.Property(x => x.TotalDebit).HasPrecision(18, 2);
        builder.Property(x => x.TotalCredit).HasPrecision(18, 2);

        // Smart Enum: TransactionType
        builder.Property(x => x.TransactionType)
            .HasConversion(
                v => v.Id,
                v => TransactionType.FromId<TransactionType>(v)!)
            .HasColumnName("TransactionTypeId")
            .IsRequired();

        // Smart Enum: Status
        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => JournalEntryStatus.FromId<JournalEntryStatus>(v)!)
            .HasColumnName("StatusId")
            .IsRequired();

        // Relationships
        builder.HasMany(x => x.Lines)
            .WithOne()
            .HasForeignKey(x => x.JournalEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(JournalEntry.Lines))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.PeriodCode);
        builder.HasIndex(x => x.EntryDate);
        builder.HasIndex(x => x.TransactionType);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.ReferenceId);
    }
}