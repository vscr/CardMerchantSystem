using HSM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HSM.Infrastructure.Persistence;

public class HSMDbContext : DbContext
{
    public HSMDbContext(DbContextOptions<HSMDbContext> options) : base(options)
    {
    }

    public DbSet<HSMDevice> HSMDevices => Set<HSMDevice>();
    public DbSet<HSMKey> HSMKeys => Set<HSMKey>();
    public DbSet<HSMCommandLog> HSMCommandLogs => Set<HSMCommandLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HSMDbContext).Assembly);
    }
}