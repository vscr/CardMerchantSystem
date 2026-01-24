
using CardMerchantSystem.Shared.Data;
using Merchant.Domain.Entities;
using Merchant.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Merchant.Infrastructure.Persistence;

public class MerchantDbContext : DbContext
{
    private DatabaseProvider _provider = DatabaseProvider.SqlServer;

    public DbSet<MerchantAggregate> Merchants { get; set; } = null!;
    public DbSet<Terminal> Terminals { get; set; } = null!;

    public MerchantDbContext(DbContextOptions<MerchantDbContext> options, IConfiguration configuration)
        : base(options)
    {
        var databaseOptions = configuration
            .GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>();

        if (databaseOptions != null)
        {
            _provider = databaseOptions.Provider;
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // PostgreSQL için özel ayarlar
        if (_provider == DatabaseProvider.PostgreSql)
        {
            // Timestamp davranışını düzelt
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            // Guid'leri UUID olarak map et
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var schema = _provider == DatabaseProvider.PostgreSql ? "public" : "dbo";

        // Configuration'ları uygula
        modelBuilder.ApplyConfiguration(new MerchantConfiguration(schema));
        modelBuilder.ApplyConfiguration(new TerminalConfiguration(schema));
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        // PostgreSQL için Guid -> UUID mapping
        if (_provider == DatabaseProvider.PostgreSql)
        {
            configurationBuilder
                .Properties<Guid>()
                .HaveColumnType("uuid");

            configurationBuilder
                .Properties<DateTime>()
                .HaveColumnType("timestamp without time zone");

            configurationBuilder
                .Properties<DateTimeOffset>()
                .HaveColumnType("timestamp with time zone");
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Metadata.FindProperty("CreatedAt") != null)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                }
            }

            if (entry.Metadata.FindProperty("UpdatedAt") != null)
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}