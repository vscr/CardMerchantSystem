using Microsoft.EntityFrameworkCore;
using Merchant.Infrastructure.Persistence.Configurations;

namespace Merchant.Infrastructure.Persistence;

public class MerchantDbContext_Pg : MerchantDbContextBase
{
    public MerchantDbContext_Pg(DbContextOptions<MerchantDbContext_Pg> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }

    protected override void ConfigureModel(ModelBuilder modelBuilder)
    {
        var schema = GetSchema();
        modelBuilder.ApplyConfiguration(new MerchantConfiguration(schema));
        modelBuilder.ApplyConfiguration(new TerminalConfiguration(schema));
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<Guid>().HaveColumnType("uuid");
        configurationBuilder.Properties<DateTime>().HaveColumnType("timestamp without time zone");
        configurationBuilder.Properties<DateTimeOffset>().HaveColumnType("timestamp with time zone");
    }

    protected override string GetSchema() => "public";
}