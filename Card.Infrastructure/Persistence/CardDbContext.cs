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

        // Tüm configuration'ları otomatik uygula
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CardDbContext).Assembly);
    }
}