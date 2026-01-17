using MerchantSettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MerchantSettlement.Infrastructure.Persistence;

public class MerchantSettlementDbContext : DbContext
{
    public MerchantSettlementDbContext(DbContextOptions<MerchantSettlementDbContext> options)
        : base(options)
    {
    }

    public DbSet<SettlementBatch> SettlementBatches => Set<SettlementBatch>();
    public DbSet<SettlementDetail> SettlementDetails => Set<SettlementDetail>();
    public DbSet<MerchantPayout> MerchantPayouts => Set<MerchantPayout>();
    public DbSet<SettlementReconciliation> SettlementReconciliations => Set<SettlementReconciliation>();
    public DbSet<ReconciliationMismatch> ReconciliationMismatches => Set<ReconciliationMismatch>();
    public DbSet<DailySettlementSummary> DailySettlementSummaries => Set<DailySettlementSummary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MerchantSettlementDbContext).Assembly);
    }
}