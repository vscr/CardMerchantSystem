using Fraud.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fraud.Infrastructure.Configurations;

public class HitScenarioConfiguration : IEntityTypeConfiguration<HitScenario>
{
    public void Configure(EntityTypeBuilder<HitScenario> builder)
    {
        builder.ToTable("HitScenarios", "fraud");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MaskedCardNo).HasMaxLength(20).IsRequired();
        builder.Property(x => x.MerchantId).HasMaxLength(50);
        builder.Property(x => x.FraudResponseCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Score);
        builder.Property(x => x.ExecutionTimeMs);

        builder.HasIndex(x => x.TransactionId);
        builder.HasIndex(x => x.MaskedCardNo);
        builder.HasIndex(x => x.FraudScenarioId);
        builder.HasIndex(x => x.DetectedAt);

        builder.HasOne(x => x.FraudScenario)
            .WithMany()
            .HasForeignKey(x => x.FraudScenarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}