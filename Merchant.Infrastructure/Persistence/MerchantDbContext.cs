using MediatR;
using Merchant.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Merchant.Infrastructure.Persistence;

public class MerchantDbContext : MerchantDbContextBase
{
    public MerchantDbContext(DbContextOptions<MerchantDbContext> options, IMediator? mediator)
      : base(options, mediator)
    {
    }

    protected override void ConfigureModel(ModelBuilder modelBuilder)
    {
        var schema = GetSchema();
        modelBuilder.ApplyConfiguration(new MerchantConfiguration(schema));
        modelBuilder.ApplyConfiguration(new TerminalConfiguration(schema));
    }

    protected override string GetSchema() => "dbo";
}