using Statement.Domain.Entities;
using Statement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Statement.Infrastructure.Configurations;

public class CardStatementConfiguration : IEntityTypeConfiguration<CardStatement>
{
    public void Configure(EntityTypeBuilder<CardStatement> builder)
    {
        builder.ToTable("CardStatements");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.StatementNumber).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.StatementNumber).IsUnique();

        builder.Property(x => x.CardNumber).HasMaxLength(16).IsRequired();
        builder.Property(x => x.MaskedCardNumber).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CustomerName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CustomerEmail).HasMaxLength(100);
        builder.Property(x => x.CustomerPhone).HasMaxLength(20);
        builder.Property(x => x.PdfPath).HasMaxLength(500);
        builder.Property(x => x.CreatedBy).HasMaxLength(50);
        builder.Property(x => x.UpdatedBy).HasMaxLength(50);

        // Decimal properties
        builder.Property(x => x.PreviousBalance).HasPrecision(18, 2);
        builder.Property(x => x.TotalDebits).HasPrecision(18, 2);
        builder.Property(x => x.TotalCredits).HasPrecision(18, 2);
        builder.Property(x => x.CurrentBalance).HasPrecision(18, 2);
        builder.Property(x => x.MinimumPayment).HasPrecision(18, 2);
        builder.Property(x => x.AvailableCredit).HasPrecision(18, 2);
        builder.Property(x => x.CreditLimit).HasPrecision(18, 2);
        builder.Property(x => x.InterestRate).HasPrecision(10, 4);
        builder.Property(x => x.InterestAmount).HasPrecision(18, 2);
        builder.Property(x => x.CashAdvanceInterestRate).HasPrecision(10, 4);
        builder.Property(x => x.PaidAmount).HasPrecision(18, 2);

        // Smart Enum: Status
        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => StatementStatus.FromId<StatementStatus>(v)!)
            .HasColumnName("StatusId")
            .IsRequired();

        // Smart Enum: PaymentStatus
        builder.Property(x => x.PaymentStatus)
            .HasConversion(
                v => v.Id,
                v => PaymentStatus.FromId<PaymentStatus>(v)!)
            .HasColumnName("PaymentStatusId")
            .IsRequired();

        // Relationships
        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.StatementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(CardStatement.Items))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.CardNumber);
        builder.HasIndex(x => x.DueDate);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.PaymentStatus);
    }
}