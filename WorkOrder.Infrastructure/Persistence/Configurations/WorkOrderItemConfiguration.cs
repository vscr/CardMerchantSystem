using CardMerchantSystem.Shared.Kernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkOrder.Domain.Entities;
using WorkOrder.Domain.Enums;

namespace WorkOrder.Infrastructure.Persistence.Configurations;

public class WorkOrderItemConfiguration : IEntityTypeConfiguration<WorkOrderItem>
{
    public void Configure(EntityTypeBuilder<WorkOrderItem> builder)
    {
        builder.ToTable("WorkOrderItems");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => x.OrderNumber).IsUnique();

        builder.HasIndex(x => x.WorkOrderTypeId);
        builder.HasIndex(x => x.CardId);
        builder.HasIndex(x => x.CustomerId);

        builder.Property(x => x.CustomerName).HasMaxLength(100);
        builder.Property(x => x.CustomerPhone).HasMaxLength(20);
        builder.Property(x => x.Subject).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.AssignedTo).HasMaxLength(100);
        builder.Property(x => x.AssignedTeam).HasMaxLength(100);
        builder.Property(x => x.CompletedBy).HasMaxLength(100);
        builder.Property(x => x.Resolution).HasMaxLength(2000);

        builder.Property(x => x.Status)
            .HasConversion(v => v.Id, v => Enumeration.FromId<WorkOrderStatus>(v)!)
            .HasColumnName("StatusId");

        builder.Property(x => x.Priority)
            .HasConversion(v => v.Id, v => Enumeration.FromId<WorkOrderPriority>(v)!)
            .HasColumnName("PriorityId");

        builder.HasOne(x => x.Type)
            .WithMany()
            .HasForeignKey(x => x.WorkOrderTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Notes)
            .WithOne()
            .HasForeignKey(x => x.WorkOrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Approvals)
            .WithOne()
            .HasForeignKey(x => x.WorkOrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.UpdatedBy).HasMaxLength(100);
    }
}