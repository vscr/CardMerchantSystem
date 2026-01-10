using HSM.Domain.Entities;
using HSM.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HSM.Infrastructure.Configurations;

public class HSMCommandLogConfiguration : IEntityTypeConfiguration<HSMCommandLog>
{
    public void Configure(EntityTypeBuilder<HSMCommandLog> builder)
    {
        builder.ToTable("HSMCommandLogs");

        builder.HasKey(x => x.Id);

        // String properties
        builder.Property(x => x.RequestData).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.ResponseData).HasMaxLength(2000);
        builder.Property(x => x.ResponseCode).HasMaxLength(10);
        builder.Property(x => x.ErrorMessage).HasMaxLength(500);
        builder.Property(x => x.ReferenceId).HasMaxLength(50);
        builder.Property(x => x.CardNumberMasked).HasMaxLength(25);

        // Smart Enum: CommandType
        builder.Property(x => x.CommandType)
            .HasConversion(
                v => v.Id,
                v => HSMCommandType.FromId<HSMCommandType>(v)!)
            .HasColumnName("CommandTypeId")
            .IsRequired();

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);

        // Indexes
        builder.HasIndex(x => x.HSMDeviceId);
        builder.HasIndex(x => x.ExecutedAt);
        builder.HasIndex(x => x.IsSuccess);
        builder.HasIndex(x => x.ReferenceId);
    }
}