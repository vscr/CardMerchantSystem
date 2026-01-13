using Fee.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fee.Infrastructure.Persistence;

public class FeeDbContext : DbContext
{
    public FeeDbContext(DbContextOptions<FeeDbContext> options) : base(options)
    {
    }

    public DbSet<Tariff> Tariffs => Set<Tariff>();
    public DbSet<TariffRule> TariffRules => Set<TariffRule>();
    public DbSet<MerchantTariff> MerchantTariffs => Set<MerchantTariff>();
    public DbSet<FeeAccrual> FeeAccruals => Set<FeeAccrual>();
    public DbSet<MembershipFee> MembershipFees => Set<MembershipFee>();
    public DbSet<CommissionBreakdown> CommissionBreakdowns => Set<CommissionBreakdown>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FeeDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Yeni Rule kayıtlarını kontrol et
        foreach (var entry in ChangeTracker.Entries<TariffRule>())
        {
            if (entry.State == EntityState.Modified)
            {
                var exists = await TariffRules
                    .AnyAsync(x => x.Id == entry.Entity.Id, cancellationToken);

                if (!exists)
                {
                    entry.State = EntityState.Added;
                }
            }
        }

        // Yeni MerchantTariff kayıtlarını kontrol et
        foreach (var entry in ChangeTracker.Entries<MerchantTariff>())
        {
            if (entry.State == EntityState.Modified)
            {
                var exists = await MerchantTariffs
                    .AnyAsync(x => x.Id == entry.Entity.Id, cancellationToken);

                if (!exists)
                {
                    entry.State = EntityState.Added;
                }
            }
        }

        // Yeni FeeAccrual kayıtlarını kontrol et
        foreach (var entry in ChangeTracker.Entries<FeeAccrual>())
        {
            if (entry.State == EntityState.Modified)
            {
                var exists = await FeeAccruals
                    .AnyAsync(x => x.Id == entry.Entity.Id, cancellationToken);

                if (!exists)
                {
                    entry.State = EntityState.Added;
                }
            }
        }

        // Yeni MembershipFee kayıtlarını kontrol et
        foreach (var entry in ChangeTracker.Entries<MembershipFee>())
        {
            if (entry.State == EntityState.Modified)
            {
                var exists = await MembershipFees
                    .AnyAsync(x => x.Id == entry.Entity.Id, cancellationToken);

                if (!exists)
                {
                    entry.State = EntityState.Added;
                }
            }
        }

        // Yeni MembershipFee kayıtlarını kontrol et
        foreach (var entry in ChangeTracker.Entries<CommissionBreakdown>())
        {
            if (entry.State == EntityState.Modified)
            {
                var exists = await CommissionBreakdowns
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