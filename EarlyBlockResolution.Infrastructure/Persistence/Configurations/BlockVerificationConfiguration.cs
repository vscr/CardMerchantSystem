using CardMerchantSystem.Shared.Kernel;
using EarlyBlockResolution.Domain.Entities;
using EarlyBlockResolution.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EarlyBlockResolution.Infrastructure.Persistence.Configurations;

public class BlockVerificationConfiguration : IEntityTypeConfiguration<BlockVerification>
{
    public void Configure(EntityTypeBuilder<BlockVerification> builder)
    {
        builder.ToTable("BlockVerifications");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.CardBlockId);

        builder.Property(x => x.OtpCode)
            .HasMaxLength(10);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.AgentUsername)
            .HasMaxLength(100);

        builder.Property(x => x.Method)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<VerificationMethod>(v)!)
            .HasColumnName("MethodId");

        builder.Property(x => x.VerificationResult)
            .HasConversion(
                v => v.Id,
                v => Enumeration.FromId<VerificationResult>(v)!)
            .HasColumnName("ResultId");
    }
}