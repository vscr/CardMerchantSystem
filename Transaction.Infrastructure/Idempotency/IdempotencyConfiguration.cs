using CardMerchantSystem.Shared.Idempotency;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Transaction.Infrastructure.Idempotency;

public class IdempotencyConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.ToTable("IdempotencyRecords");

        // PK: IdempotencyKey (string — doğal key, surrogate ID'ye gerek yok)
        builder.HasKey(x => x.IdempotencyKey);
        builder.Property(x => x.IdempotencyKey).HasMaxLength(200);

        builder.Property(x => x.RequestPath).HasMaxLength(500).IsRequired();
        builder.Property(x => x.RequestHash).HasMaxLength(64).IsRequired(); // SHA256 = 64 char hex
        builder.Property(x => x.ResponseBody).HasColumnType("nvarchar(max)");
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();

        // Index'ler
        builder.HasIndex(x => x.ExpiresAt);                    // Cleanup job için
        builder.HasIndex(x => x.TransactionId);                // Transaction ile ilişkilendirme
        builder.HasIndex(x => new { x.Status, x.CreatedAt });  // Monitoring
    }
}