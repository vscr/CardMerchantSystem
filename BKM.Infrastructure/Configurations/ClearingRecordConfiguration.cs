using BKM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BKM.Infrastructure.Configurations;

public class ClearingRecordConfiguration : IEntityTypeConfiguration<ClearingRecord>
{
    public void Configure(EntityTypeBuilder<ClearingRecord> builder)
    {
        builder.ToTable("ClearingRecords");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.STAN).HasMaxLength(6).IsRequired();
        builder.Property(x => x.RRN).HasMaxLength(12).IsRequired();
        builder.Property(x => x.AuthorizationCode).HasMaxLength(6).IsRequired();
        builder.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        builder.Property(x => x.AcquirerBankCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.IssuerBankCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.MerchantId).HasMaxLength(15).IsRequired();
        builder.Property(x => x.TerminalId).HasMaxLength(8).IsRequired();
        builder.Property(x => x.CardNumberMasked).HasMaxLength(25).IsRequired();
        builder.Property(x => x.BIN).HasMaxLength(6).IsRequired();
        builder.Property(x => x.ClearingDate).HasMaxLength(8).IsRequired();

        // Decimal properties
        builder.Property(x => x.TransactionAmount).HasPrecision(18, 2);
        builder.Property(x => x.ClearingAmount).HasPrecision(18, 2);
        builder.Property(x => x.FeeAmount).HasPrecision(18, 2);
        builder.Property(x => x.NetAmount).HasPrecision(18, 2);

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.SwitchMessageId);
        builder.HasIndex(x => x.STAN);
        builder.HasIndex(x => x.ClearingDate);
        builder.HasIndex(x => x.IsSettled);
        builder.HasIndex(x => x.AcquirerBankCode);
        builder.HasIndex(x => x.IssuerBankCode);
    }
}