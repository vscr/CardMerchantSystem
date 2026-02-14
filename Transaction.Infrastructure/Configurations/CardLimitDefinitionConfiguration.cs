using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transaction.Domain.Entities;

namespace Transaction.Infrastructure.Configurations;

public class CardLimitDefinitionConfiguration : IEntityTypeConfiguration<CardLimitDefinition>
{
    public void Configure(EntityTypeBuilder<CardLimitDefinition> builder)
    {
        builder.ToTable("CardLimitDefinitions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.LimitType).HasMaxLength(20).IsRequired();
        builder.Property(x => x.TargetValue).HasMaxLength(50);
        builder.Property(x => x.DailyLimit).HasPrecision(18, 2);
        builder.Property(x => x.MonthlyLimit).HasPrecision(18, 2);
        builder.Property(x => x.SingleTransactionLimit).HasPrecision(18, 2);
        builder.Property(x => x.Currency).HasMaxLength(5).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.UpdatedBy).HasMaxLength(100);

        builder.HasIndex(x => new { x.LimitType, x.TargetValue }).IsUnique();
        builder.HasIndex(x => x.IsActive);

        // Seed: Default limit
        builder.HasData(new
        {
            Id = Guid.Parse("E0000001-0000-0000-0000-000000000001"),
            LimitType = "DEFAULT",
            DailyLimit = 10000m,
            MonthlyLimit = 50000m,
            SingleTransactionLimit = (decimal?)5000m,
            Currency = "TRY",
            Description = (string?)"Sistem varsayılan limiti",
            IsActive = true,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            CreatedBy = (string?)"system"
        });
    }
}