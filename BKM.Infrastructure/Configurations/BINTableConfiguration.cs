using BKM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BKM.Infrastructure.Configurations;

public class BINTableConfiguration : IEntityTypeConfiguration<BINTable>
{
    public void Configure(EntityTypeBuilder<BINTable> builder)
    {
        builder.ToTable("BINTables");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.BIN).HasMaxLength(6).IsRequired();
        builder.HasIndex(x => x.BIN).IsUnique();

        builder.Property(x => x.BankCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.BankName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CardBrand).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CardType).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CardLevel).HasMaxLength(20).IsRequired();

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.BankCode);
        builder.HasIndex(x => x.CardBrand);
        builder.HasIndex(x => x.IsActive);
    }
}