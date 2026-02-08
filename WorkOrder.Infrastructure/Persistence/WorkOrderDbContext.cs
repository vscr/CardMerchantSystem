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

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // WorkOrderNote kayıtlarını kontrol et - yeni olanları Added olarak işaretle
        foreach (var entry in ChangeTracker.Entries<WorkOrderNote>())
        {
            if (entry.State == EntityState.Modified)
            {
                // Veritabanında var mı kontrol et
                var exists = await WorkOrderNotes
                    .AnyAsync(x => x.Id == entry.Entity.Id, cancellationToken);

                if (!exists)
                {
                    entry.State = EntityState.Added;
                }
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }
}