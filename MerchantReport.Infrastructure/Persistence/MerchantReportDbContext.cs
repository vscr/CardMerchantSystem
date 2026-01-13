using MerchantReport.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MerchantReport.Infrastructure.Persistence;

public class MerchantReportDbContext : DbContext
{
    public MerchantReportDbContext(DbContextOptions<MerchantReportDbContext> options) : base(options)
    {
    }

    public DbSet<MerchantReportConfig> MerchantReportConfigs => Set<MerchantReportConfig>();
    public DbSet<ReportRequest> ReportRequests => Set<ReportRequest>();
    public DbSet<MerchantStatement> MerchantStatements => Set<MerchantStatement>();
    public DbSet<MerchantStatementItem> MerchantStatementItems => Set<MerchantStatementItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MerchantReportDbContext).Assembly);
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // MerchantStatementItem kayıtlarını kontrol et - yeni olanları Added olarak işaretle
        foreach (var entry in ChangeTracker.Entries<MerchantStatementItem>())
        {
            if (entry.State == EntityState.Modified)
            {
                var exists = await MerchantStatementItems
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