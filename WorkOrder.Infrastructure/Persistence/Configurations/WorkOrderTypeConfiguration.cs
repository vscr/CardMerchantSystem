using CardMerchantSystem.Shared.Kernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkOrder.Domain.Entities;
using WorkOrder.Domain.Enums;

namespace WorkOrder.Infrastructure.Persistence.Configurations;

public class WorkOrderTypeConfiguration : IEntityTypeConfiguration<WorkOrderType>
{
    public void Configure(EntityTypeBuilder<WorkOrderType> builder)
    {
        builder.ToTable("WorkOrderTypes");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => x.Code).IsUnique();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(500);

        builder.Property(x => x.Category)
            .HasConversion(v => v.Id, v => Enumeration.FromId<WorkOrderCategory>(v)!)
            .HasColumnName("CategoryId");

        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.UpdatedBy).HasMaxLength(100);
    }
}