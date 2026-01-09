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
}