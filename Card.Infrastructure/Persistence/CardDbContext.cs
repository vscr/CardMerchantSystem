using Card.Domain.Entities;
using CardMerchantSystem.Shared.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Card.Infrastructure.Persistence;

public class CardDbContext : DbContext
{
    private readonly IMediator? _mediator;
    public CardDbContext(DbContextOptions<CardDbContext> options, IMediator? mediator) : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<CardApplication> CardApplications => Set<CardApplication>();
    public DbSet<CardApplicationStatusHistory> CardApplicationStatusHistories => Set<CardApplicationStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CardDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // StatusHistory kayıtlarını kontrol et - yeni olanları Added olarak işaretle
        foreach (var entry in ChangeTracker.Entries<CardApplicationStatusHistory>())
        {
            if (entry.State == EntityState.Modified)
            {
                // Veritabanında var mı kontrol et
                var exists = await CardApplicationStatusHistories
                    .AnyAsync(x => x.Id == entry.Entity.Id, cancellationToken);

                if (!exists)
                {
                    entry.State = EntityState.Added;
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