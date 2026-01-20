using MerchantSettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MerchantSettlement.Infrastructure.Persistence;

public class MerchantSettlementDbContext : DbContext
{
    public MerchantSettlementDbContext(DbContextOptions<MerchantSettlementDbContext> options)
        : base(options)
    {
    }

    public DbSet<MerchantSettlementBatch> MerchantSettlementBatches => Set<MerchantSettlementBatch>();
    public DbSet<MerchantSettlementDetail> MerchantSettlementDetails => Set<MerchantSettlementDetail>();
    public DbSet<MerchantPayout> MerchantPayouts => Set<MerchantPayout>();
    public DbSet<MerchantReconciliation> MerchantSettlementReconciliations => Set<MerchantReconciliation>();
    public DbSet<MerchantReconciliationMismatch> MerchantReconciliationMismatches => Set<MerchantReconciliationMismatch>();
    public DbSet<MerchantDailySettlementSummary> MerchantDailySettlementSummaries => Set<MerchantDailySettlementSummary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MerchantSettlementDbContext).Assembly);
    }
}