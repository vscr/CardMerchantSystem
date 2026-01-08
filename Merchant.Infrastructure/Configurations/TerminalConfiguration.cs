using Merchant.Domain.Entities;
using Merchant.Domain.Enums;
using Merchant.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Merchant.Infrastructure.Configurations;

public class TerminalConfiguration : IEntityTypeConfiguration<Terminal>
{
    public void Configure(EntityTypeBuilder<Terminal> builder)
    {
        builder.ToTable("Terminals");

        builder.HasKey(x => x.Id);

        // Value Object: TerminalCode
        builder.OwnsOne(x => x.TerminalCode, tc =>
        {
            tc.Property(t => t.Value)
                .HasColumnName("TerminalCode")
                .HasMaxLength(8)
                .IsRequired();

            tc.HasIndex(t => t.Value).IsUnique();
        });

        // Smart Enum: TerminalType
        builder.Property(x => x.TerminalType)
            .HasConversion(
                v => v.Id,
                v => TerminalType.FromId<TerminalType>(v)!)
            .HasColumnName("TerminalTypeId")
            .IsRequired();

        // Smart Enum: Status
        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => TerminalStatus.FromId<TerminalStatus>(v)!)
            .HasColumnName("StatusId")
            .IsRequired();

        // String properties
        builder.Property(x => x.SerialNumber).HasMaxLength(50);
        builder.Property(x => x.Model).HasMaxLength(50);
        builder.Property(x => x.Location).HasMaxLength(200);
        builder.Property(x => x.InstalledBy).HasMaxLength(50);

        // Indexes
        builder.HasIndex(x => x.MerchantId);
    }
}