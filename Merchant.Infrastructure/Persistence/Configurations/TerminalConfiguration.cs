using Merchant.Domain.Entities;
using Merchant.Domain.Enums;
using Merchant.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Merchant.Infrastructure.Persistence.Configurations;

public class TerminalConfiguration : IEntityTypeConfiguration<Terminal>
{
    private readonly string _schema;

    public TerminalConfiguration(string schema = "dbo")
    {
        _schema = schema;
    }

    public void Configure(EntityTypeBuilder<Terminal> builder)
    {
        builder.ToTable("Terminals", _schema);

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.MerchantId)
            .IsRequired();

        // Value Object: TerminalCode (TerminalId)
        builder.OwnsOne(t => t.TerminalCode, code =>
        {
            code.Property(c => c.Value)
                .HasColumnName("TerminalCode")
                .HasMaxLength(20)
                .IsRequired();
            code.HasIndex(c => c.Value).IsUnique();
        });

        // Smart Enum: TerminalType
        builder.Property(t => t.TerminalType)
            .HasConversion(
                v => v.Id,
                v => TerminalType.FromId<TerminalType>(v)!)
            .HasColumnName("TerminalTypeId")
            .IsRequired();

        // Smart Enum: Status
        builder.Property(t => t.Status)
            .HasConversion(
                v => v.Id,
                v => TerminalStatus.FromId<TerminalStatus>(v)!)
            .HasColumnName("StatusId")
            .IsRequired();

        // Terminal Bilgileri
        builder.Property(t => t.SerialNumber)
            .HasMaxLength(50);

        builder.Property(t => t.Model)
            .HasMaxLength(50);

        builder.Property(t => t.Location)
            .HasMaxLength(200);

        // Kurulum Bilgileri
        builder.Property(t => t.InstalledAt);

        builder.Property(t => t.InstalledBy)
            .HasMaxLength(50);

        // Audit
        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt);

        // Indexes
        builder.HasIndex(t => t.MerchantId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.CreatedAt);
    }
}