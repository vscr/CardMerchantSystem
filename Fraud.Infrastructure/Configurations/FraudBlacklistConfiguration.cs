using Fraud.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fraud.Infrastructure.Configurations;

public class FraudBlacklistConfiguration : IEntityTypeConfiguration<FraudBlacklist>
{
    public void Configure(EntityTypeBuilder<FraudBlacklist> builder)
    {
        builder.ToTable("FraudBlacklists", "fraud");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ListType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Reason).HasMaxLength(500);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);

        builder.HasIndex(x => new { x.ListType, x.Value }).IsUnique();
        builder.HasIndex(x => new { x.ListType, x.IsBlacklist, x.IsActive });
    }
}