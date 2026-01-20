using BulkCardPrint.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BulkCardPrint.Infrastructure.Persistence;

public class BulkCardPrintDbContext : DbContext
{
    public BulkCardPrintDbContext(DbContextOptions<BulkCardPrintDbContext> options)
        : base(options)
    {
    }

    public DbSet<PrintVendor> PrintVendors => Set<PrintVendor>();
    public DbSet<PrintBatch> PrintBatches => Set<PrintBatch>();
    public DbSet<PrintBatchItem> PrintBatchItems => Set<PrintBatchItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BulkCardPrintDbContext).Assembly);
    }
}