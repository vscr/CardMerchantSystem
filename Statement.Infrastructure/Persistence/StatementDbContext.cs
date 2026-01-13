using Statement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Statement.Infrastructure.Persistence;

public class StatementDbContext : DbContext
{
    public StatementDbContext(DbContextOptions<StatementDbContext> options) : base(options)
    {
    }

    public DbSet<CardStatement> CardStatements => Set<CardStatement>();
    public DbSet<StatementItem> StatementItems => Set<StatementItem>();
    public DbSet<StatementNotification> StatementNotifications => Set<StatementNotification>();
    public DbSet<StatementPeriodConfig> StatementPeriodConfigs => Set<StatementPeriodConfig>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StatementDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // StatementItem kayıtlarını kontrol et - yeni olanları Added olarak işaretle
        foreach (var entry in ChangeTracker.Entries<StatementItem>())
        {
            if (entry.State == EntityState.Modified)
            {
                var exists = await StatementItems
                    .AnyAsync(x => x.Id == entry.Entity.Id, cancellationToken);

                if (!exists)
                {
                    entry.State = EntityState.Added;
                }
            }
        }

        // StatementNotification kayıtlarını kontrol et - yeni olanları Added olarak işaretle
        foreach (var entry in ChangeTracker.Entries<StatementNotification>())
        {
            if (entry.State == EntityState.Modified)
            {
                var exists = await StatementNotifications
                    .AnyAsync(x => x.Id == entry.Entity.Id, cancellationToken);

                if (!exists)
                {
                    entry.State = EntityState.Added;
                }
            }
        }

        // StatementPeriodConfigs kayıtlarını kontrol et - yeni olanları Added olarak işaretle
        foreach (var entry in ChangeTracker.Entries<StatementPeriodConfig>())
        {
            if (entry.State == EntityState.Modified)
            {
                var exists = await StatementPeriodConfigs
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