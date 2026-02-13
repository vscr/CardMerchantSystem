using Fraud.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fraud.Infrastructure.Configurations;

public class FraudActionConfiguration : IEntityTypeConfiguration<FraudAction>
{
    public void Configure(EntityTypeBuilder<FraudAction> builder)
    {
        builder.ToTable("FraudActions", "fraud");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MaskedCardNo).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Decision).HasConversion<int>();
        builder.Property(x => x.Comment).HasMaxLength(2000);
        builder.Property(x => x.CardStatusAction).HasMaxLength(50);
        builder.Property(x => x.CardStatusReasonCode).HasMaxLength(50);
        builder.Property(x => x.ActionBy).HasMaxLength(100).IsRequired();

        builder.HasIndex(x => x.FraudAlertId);
        builder.HasIndex(x => x.TransactionId);
        builder.HasIndex(x => x.MaskedCardNo);

        builder.HasOne(x => x.FraudAlert)
            .WithMany()
            .HasForeignKey(x => x.FraudAlertId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}