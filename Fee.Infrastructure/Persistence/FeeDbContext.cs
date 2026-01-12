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
}