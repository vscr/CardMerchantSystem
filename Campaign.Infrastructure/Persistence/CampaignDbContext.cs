using Campaign.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Campaign.Infrastructure.Persistence;

public class CampaignDbContext : DbContext
{
    public CampaignDbContext(DbContextOptions<CampaignDbContext> options) : base(options)
    {
    }

    public DbSet<CampaignAggregate> Campaigns => Set<CampaignAggregate>();
    public DbSet<CampaignRule> CampaignRules => Set<CampaignRule>();
    public DbSet<CampaignUsage> CampaignUsages => Set<CampaignUsage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CampaignDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Yeni Rule kayıtlarını kontrol et
        foreach (var entry in ChangeTracker.Entries<CampaignRule>())
        {
            if (entry.State == EntityState.Modified)
            {
                var exists = await CampaignRules
                    .AnyAsync(x => x.Id == entry.Entity.Id, cancellationToken);

                if (!exists)
                {
                    entry.State = EntityState.Added;
                }
            }
        }

        // Yeni Usage kayıtlarını kontrol et
        foreach (var entry in ChangeTracker.Entries<CampaignUsage>())
        {
            if (entry.State == EntityState.Modified)
            {
                var exists = await CampaignUsages
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