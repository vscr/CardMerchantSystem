using Microsoft.EntityFrameworkCore;
using WorkOrder.Domain.Entities;

namespace WorkOrder.Infrastructure.Persistence;

public class WorkOrderDbContext : DbContext
{
    public WorkOrderDbContext(DbContextOptions<WorkOrderDbContext> options)
        : base(options)
    {
    }

    public DbSet<WorkOrderType> WorkOrderTypes => Set<WorkOrderType>();
    public DbSet<WorkOrderItem> WorkOrderItems => Set<WorkOrderItem>();
    public DbSet<WorkOrderNote> WorkOrderNotes => Set<WorkOrderNote>();
    public DbSet<WorkOrderApproval> WorkOrderApprovals => Set<WorkOrderApproval>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkOrderDbContext).Assembly);
    }
}