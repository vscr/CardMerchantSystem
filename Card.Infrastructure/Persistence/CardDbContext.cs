using Card.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Card.Infrastructure.Persistence;

public class CardDbContext : DbContext
{
    public CardDbContext(DbContextOptions<CardDbContext> options) : base(options)
    {
    }

    public DbSet<CardApplication> CardApplications => Set<CardApplication>();
    public DbSet<CardApplicationStatusHistory> CardApplicationStatusHistories => Set<CardApplicationStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CardDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // StatusHistory kayıtlarını kontrol et - yeni olanları Added olarak işaretle
        foreach (var entry in ChangeTracker.Entries<CardApplicationStatusHistory>())
        {
            if (entry.State == EntityState.Modified)
            {
                // Veritabanında var mı kontrol et
                var exists = await CardApplicationStatusHistories
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