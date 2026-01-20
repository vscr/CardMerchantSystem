using CardMerchantSystem.Shared.Kernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkOrder.Domain.Entities;
using WorkOrder.Domain.Enums;

namespace WorkOrder.Infrastructure.Persistence.Configurations;

public class WorkOrderApprovalConfiguration : IEntityTypeConfiguration<WorkOrderApproval>
{
    public void Configure(EntityTypeBuilder<WorkOrderApproval> builder)
    {
        builder.ToTable("WorkOrderApprovals");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.WorkOrderItemId);

        builder.Property(x => x.ApproverUsername).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Notes).HasMaxLength(1000);

        builder.Property(x => x.Status)
            .HasConversion(v => v.Id, v => Enumeration.FromId<ApprovalStatus>(v)!)
            .HasColumnName("StatusId");
    }
}