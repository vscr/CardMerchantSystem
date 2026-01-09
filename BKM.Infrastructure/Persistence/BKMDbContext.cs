using BKM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BKM.Infrastructure.Persistence;

public class BKMDbContext : DbContext
{
    public BKMDbContext(DbContextOptions<BKMDbContext> options) : base(options)
    {
    }

    public DbSet<SwitchMessage> SwitchMessages => Set<SwitchMessage>();
    public DbSet<ClearingRecord> ClearingRecords => Set<ClearingRecord>();
    public DbSet<SettlementBatch> SettlementBatches => Set<SettlementBatch>();
    public DbSet<BankSettlementSummary> BankSettlementSummaries => Set<BankSettlementSummary>();
    public DbSet<BINTable> BINTables => Set<BINTable>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BKMDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Yeni BankSettlementSummary kayıtlarını kontrol et
        foreach (var entry in ChangeTracker.Entries<BankSettlementSummary>())
        {
            if (entry.State == EntityState.Modified)
            {
                var exists = await BankSettlementSummaries
                    .AnyAsync(x => x.Id == entry.Entity.Id, cancellationToken);

                if (!exists)
                {
                    entry.State = EntityState.Added;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}