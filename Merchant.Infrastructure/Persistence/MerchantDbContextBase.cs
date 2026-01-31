using CardMerchantSystem.Shared.Extensions;
using MediatR;
using Merchant.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Merchant.Infrastructure.Persistence;

public abstract class MerchantDbContextBase : DbContext
{
    protected readonly IMediator? _mediator;
    protected MerchantDbContextBase(DbContextOptions options, IMediator? mediator) : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<MerchantAggregate> Merchants => Set<MerchantAggregate>();
    public DbSet<Terminal> Terminals => Set<Terminal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureModel(modelBuilder);
    }

    protected abstract void ConfigureModel(ModelBuilder modelBuilder);
    protected abstract string GetSchema();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
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

        var result = await base.SaveChangesAsync(cancellationToken);

        if (_mediator != null)
        {
            await _mediator.DispatchDomainEventsAsync(this);
        }

        return result;
    }
}