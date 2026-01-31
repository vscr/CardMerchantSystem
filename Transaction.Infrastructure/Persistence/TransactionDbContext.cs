using CardMerchantSystem.Shared.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Transaction.Domain.Entities;

namespace Transaction.Infrastructure.Persistence;

public class TransactionDbContext : DbContext
{
    private readonly IMediator? _mediator;
    public TransactionDbContext(DbContextOptions<TransactionDbContext> options, IMediator? mediator) : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<TransactionAggregate> Transactions => Set<TransactionAggregate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TransactionDbContext).Assembly);
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {

        var result = await base.SaveChangesAsync(cancellationToken);

        if (_mediator != null)
        {
            await _mediator.DispatchDomainEventsAsync(this);
        }

        return result;
    }

}