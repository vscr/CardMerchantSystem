using Dispute.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dispute.Infrastructure.Persistence;

public class DisputeDbContext : DbContext
{
    public DisputeDbContext(DbContextOptions<DisputeDbContext> options) : base(options)
    {
    }

    public DbSet<DisputeAggregate> Disputes => Set<DisputeAggregate>();
    public DbSet<DisputeDocument> DisputeDocuments => Set<DisputeDocument>();
    public DbSet<DisputeNote> DisputeNotes => Set<DisputeNote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DisputeDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Yeni Note kayıtlarını kontrol et
        foreach (var entry in ChangeTracker.Entries<DisputeNote>())
        {
            if (entry.State == EntityState.Modified)
            {
                var exists = await DisputeNotes
                    .AnyAsync(x => x.Id == entry.Entity.Id, cancellationToken);

                if (!exists)
                {
                    entry.State = EntityState.Added;
                }
            }
        }

        // Yeni Document kayıtlarını kontrol et
        foreach (var entry in ChangeTracker.Entries<DisputeDocument>())
        {
            if (entry.State == EntityState.Modified)
            {
                var exists = await DisputeDocuments
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