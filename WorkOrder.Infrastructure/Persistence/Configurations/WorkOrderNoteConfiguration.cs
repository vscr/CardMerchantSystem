using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkOrder.Domain.Entities;

namespace WorkOrder.Infrastructure.Persistence.Configurations;

public class WorkOrderNoteConfiguration : IEntityTypeConfiguration<WorkOrderNote>
{
    public void Configure(EntityTypeBuilder<WorkOrderNote> builder)
    {
        builder.ToTable("WorkOrderNotes");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.WorkOrderItemId);

        builder.Property(x => x.Content).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.CreatedBy).IsRequired().HasMaxLength(100);
    }
}