using Transaction.Domain.Entities;
using Transaction.Domain.Enums;
using Transaction.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Transaction.Infrastructure.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<TransactionAggregate>
{
    public void Configure(EntityTypeBuilder<TransactionAggregate> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(x => x.Id);

        // Value Object: ReferenceNumber
        builder.OwnsOne(x => x.ReferenceNumber, rrn =>
        {
            rrn.Property(r => r.Value)
                .HasColumnName("ReferenceNumber")
                .HasMaxLength(12)
                .IsRequired();

            rrn.HasIndex(r => r.Value).IsUnique();
        });

        // Value Object: Amount
        builder.OwnsOne(x => x.Amount, amount =>
        {
            amount.Property(a => a.Amount)
                .HasColumnName("Amount")
                .HasPrecision(18, 2)
                .IsRequired();

            amount.Property(a => a.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Value Object: AuthorizationCode (nullable)
        builder.OwnsOne(x => x.AuthorizationCode, auth =>
        {
            auth.Property(a => a.Value)
                .HasColumnName("AuthorizationCode")
                .HasMaxLength(6);
        });

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
                v => TransactionStatus.FromId<TransactionStatus>(v)!)
            .HasColumnName("StatusId")
            .IsRequired();

        // Smart Enum: DeclineReason (nullable)
        builder.Property(x => x.DeclineReason)
            .HasConversion(
                v => v == null ? (int?)null : v.Id,
                v => v == null ? null : DeclineReason.FromId<DeclineReason>(v.Value))
            .HasColumnName("DeclineReasonId");

        // Smart Enum: FraudCheckResult (nullable)
        builder.Property(x => x.FraudCheckResult)
            .HasConversion(
                v => v == null ? (int?)null : v.Id,
                v => v == null ? null : FraudCheckResult.FromId<FraudCheckResult>(v.Value))
            .HasColumnName("FraudCheckResultId");

        // String properties
        builder.Property(x => x.CardNumberMasked).HasMaxLength(25).IsRequired();
        builder.Property(x => x.CardNumberEncrypted).HasMaxLength(500).IsRequired();
        builder.Property(x => x.MerchantCode).HasMaxLength(15).IsRequired();
        builder.Property(x => x.TerminalCode).HasMaxLength(8).IsRequired();
        builder.Property(x => x.ErrorMessage).HasMaxLength(500);
        builder.Property(x => x.BatchNumber).HasMaxLength(25);
        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.UpdatedBy).HasMaxLength(50);

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.MerchantId);
        builder.HasIndex(x => x.TerminalId);
        builder.HasIndex(x => x.CardNumberMasked);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.OriginalTransactionId);
    }
}