using BKM.Domain.Entities;
using BKM.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BKM.Infrastructure.Configurations;

public class SwitchMessageConfiguration : IEntityTypeConfiguration<SwitchMessage>
{
    public void Configure(EntityTypeBuilder<SwitchMessage> builder)
    {
        builder.ToTable("SwitchMessages");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.STAN).HasMaxLength(6).IsRequired();
        builder.Property(x => x.RRN).HasMaxLength(12).IsRequired();
        builder.Property(x => x.CardNumberMasked).HasMaxLength(25).IsRequired();
        builder.Property(x => x.CardNumberEncrypted).HasMaxLength(500).IsRequired();
        builder.Property(x => x.ExpiryDate).HasMaxLength(4).IsRequired();
        builder.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        builder.Property(x => x.TerminalId).HasMaxLength(8).IsRequired();
        builder.Property(x => x.MerchantId).HasMaxLength(15).IsRequired();
        builder.Property(x => x.MCC).HasMaxLength(4).IsRequired();
        builder.Property(x => x.AcquirerBankCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.IssuerBankCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.BIN).HasMaxLength(6).IsRequired();
        builder.Property(x => x.AuthorizationCode).HasMaxLength(6);
        builder.Property(x => x.ErrorMessage).HasMaxLength(500);
        builder.Property(x => x.RoutingKey).HasMaxLength(50);

        // Decimal properties
        builder.Property(x => x.Amount).HasPrecision(18, 2);

        // Smart Enum: MessageType
        builder.Property(x => x.MessageType)
            .HasConversion(
                v => v.Id,
                v => MessageType.FromId<MessageType>(v)!)
            .HasColumnName("MessageTypeId")
            .IsRequired();

        // Smart Enum: ProcessingCode
        builder.Property(x => x.ProcessingCode)
            .HasConversion(
                v => v.Id,
                v => ProcessingCode.FromId<ProcessingCode>(v)!)
            .HasColumnName("ProcessingCodeId")
            .IsRequired();

        // Smart Enum: Status
        builder.Property(x => x.Status)
            .HasConversion(
                v => v.Id,
                v => SwitchMessageStatus.FromId<SwitchMessageStatus>(v)!)
            .HasColumnName("StatusId")
            .IsRequired();

        // Smart Enum: ResponseCode (nullable)
        builder.Property(x => x.ResponseCode)
            .HasConversion(
                v => v == null ? (int?)null : v.Id,
                v => v == null ? null : ResponseCode.FromId<ResponseCode>(v.Value))
            .HasColumnName("ResponseCodeId");

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.STAN);
        builder.HasIndex(x => x.RRN);
        builder.HasIndex(x => x.MerchantId);
        builder.HasIndex(x => x.TransactionDateTime);
        builder.HasIndex(x => x.ReceivedAt);
    }
}