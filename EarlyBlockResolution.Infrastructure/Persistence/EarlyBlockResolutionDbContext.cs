using EarlyBlockResolution.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EarlyBlockResolution.Infrastructure.Persistence;

public class EarlyBlockResolutionDbContext : DbContext
{
    public EarlyBlockResolutionDbContext(DbContextOptions<EarlyBlockResolutionDbContext> options)
        : base(options)
    {
    }

    public DbSet<BlockRule> BlockRules => Set<BlockRule>();
    public DbSet<FraudAlert> FraudAlerts => Set<FraudAlert>();
    public DbSet<CardBlock> CardBlocks => Set<CardBlock>();
    public DbSet<BlockVerification> BlockVerifications => Set<BlockVerification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EarlyBlockResolutionDbContext).Assembly);
    }
}