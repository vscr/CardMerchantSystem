using Microsoft.EntityFrameworkCore;
using Merchant.Infrastructure.Persistence.Configurations;

namespace Merchant.Infrastructure.Persistence;

public class MerchantDbContext : MerchantDbContextBase
{
    public MerchantDbContext(DbContextOptions<MerchantDbContext> options) : base(options)
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