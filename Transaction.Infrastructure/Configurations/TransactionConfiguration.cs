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

        // ============================================
        // INDEXES - Performance Optimization
        // ============================================

        // === SIMPLE INDEXES ===
        // Primary lookup patterns
        builder.HasIndex(x => x.MerchantId)
            .HasDatabaseName("IX_Transactions_MerchantId");

        builder.HasIndex(x => x.TerminalId)
            .HasDatabaseName("IX_Transactions_TerminalId");

        builder.HasIndex(x => x.CardNumberMasked)
            .HasDatabaseName("IX_Transactions_CardNumberMasked");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_Transactions_CreatedAt");

        builder.HasIndex(x => x.OriginalTransactionId)
            .HasDatabaseName("IX_Transactions_OriginalTransactionId");

        // === COMPOSITE INDEXES (En sık kullanılan sorgu patternleri) ===

        // Pattern 1: Merchant + Date range (En yaygın call center sorgusu)
        builder.HasIndex(x => new { x.MerchantId, x.CreatedAt })
            .HasDatabaseName("IX_Transactions_MerchantId_CreatedAt")
            .IsDescending(false, true); // MerchantId ASC, CreatedAt DESC

        // Pattern 2: Card + Date range (Kart bazlı işlem sorgulama)
        builder.HasIndex(x => new { x.CardNumberMasked, x.CreatedAt })
            .HasDatabaseName("IX_Transactions_CardNumber_CreatedAt")
            .IsDescending(false, true);

        // Pattern 3: Status + Date (Pending settlement, reporting)
        builder.HasIndex(x => new { x.Status, x.CreatedAt })
            .HasDatabaseName("IX_Transactions_Status_CreatedAt")
            .IsDescending(false, true);

        // Pattern 4: Date + Status + Type (Dashboard stats, raporlama)
        builder.HasIndex(x => new { x.CreatedAt, x.Status, x.TransactionType })
            .HasDatabaseName("IX_Transactions_Date_Status_Type");

        // Pattern 5: Card + Status + Type + Date (Limit hesaplama için kritik!)
        builder.HasIndex(x => new { x.CardNumberMasked, x.Status, x.TransactionType, x.CreatedAt })
            .HasDatabaseName("IX_Transactions_Card_Status_Type_Date");

        // Pattern 6: Settlement batch lookup
        builder.HasIndex(x => new { x.BatchNumber, x.Status })
            .HasDatabaseName("IX_Transactions_BatchNumber_Status")
            .HasFilter("[BatchNumber] IS NOT NULL");

        // Pattern 7: Terminal + Date (Terminal bazlı raporlama)
        builder.HasIndex(x => new { x.TerminalId, x.CreatedAt })
            .HasDatabaseName("IX_Transactions_TerminalId_CreatedAt")
            .IsDescending(false, true);
    }
}