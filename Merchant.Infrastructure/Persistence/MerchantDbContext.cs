using Merchant.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Merchant.Infrastructure.Persistence;

public class MerchantDbContext : DbContext
{
    public MerchantDbContext(DbContextOptions<MerchantDbContext> options) : base(options)
    {
    }

    public DbSet<MerchantAggregate> Merchants => Set<MerchantAggregate>();
    public DbSet<Terminal> Terminals => Set<Terminal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MerchantDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Terminal kayıtlarını kontrol et - yeni olanları Added olarak işaretle
        foreach (var entry in ChangeTracker.Entries<Terminal>())
        {
            if (entry.State == EntityState.Modified)
            {
                var exists = await Terminals
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