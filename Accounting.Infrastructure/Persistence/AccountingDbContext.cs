using Accounting.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Persistence;

public class AccountingDbContext : DbContext
{
    public AccountingDbContext(DbContextOptions<AccountingDbContext> options) : base(options)
    {
    }

    public DbSet<ChartOfAccount> ChartOfAccounts => Set<ChartOfAccount>();
    public DbSet<AccountingPeriod> AccountingPeriods => Set<AccountingPeriod>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalEntryLine> JournalEntryLines => Set<JournalEntryLine>();
    public DbSet<AccountBalance> AccountBalances => Set<AccountBalance>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountingDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // AccountBalance kayıtlarını kontrol et - yeni olanları Added olarak işaretle
        foreach (var entry in ChangeTracker.Entries<AccountBalance>())
        {
            if (entry.State == EntityState.Modified)
            {
                var exists = await AccountBalances
                    .AnyAsync(x => x.Id == entry.Entity.Id, cancellationToken);

                if (!exists)
                {
                    entry.State = EntityState.Added;
                }
            }
        }

        // JournalEntryLine kayıtlarını kontrol et - yeni olanları Added olarak işaretle
        foreach (var entry in ChangeTracker.Entries<JournalEntryLine>())
        {
            if (entry.State == EntityState.Modified)
            {
                var exists = await JournalEntryLines
                    .AnyAsync(x => x.Id == entry.Entity.Id, cancellationToken);

                if (!exists)
                {
                    entry.State = EntityState.Added;
                }
            }
        }

        // JournalEntry kayıtlarını kontrol et - yeni olanları Added olarak işaretle
        foreach (var entry in ChangeTracker.Entries<JournalEntry>())
        {
            if (entry.State == EntityState.Modified)
            {
                var exists = await JournalEntries
                    .AnyAsync(x => x.Id == entry.Entity.Id, cancellationToken);

                if (!exists)
                {
                    entry.State = EntityState.Added;
                }
            }
        }

        // AccountingPeriod kayıtlarını kontrol et - yeni olanları Added olarak işaretle
        foreach (var entry in ChangeTracker.Entries<AccountingPeriod>())
        {
            if (entry.State == EntityState.Modified)
            {
                var exists = await AccountingPeriods
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