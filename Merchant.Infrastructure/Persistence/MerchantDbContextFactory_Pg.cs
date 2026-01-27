using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Merchant.Infrastructure.Persistence;

public class MerchantDbContextFactory_Pg : IDesignTimeDbContextFactory<MerchantDbContext_Pg>
{
    public MerchantDbContext_Pg CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../CardMerchantSystem.API"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration["Database:PostgreSqlConnection"];

        var optionsBuilder = new DbContextOptionsBuilder<MerchantDbContext_Pg>();
        optionsBuilder.UseNpgsql(connectionString);

        return new MerchantDbContext_Pg(optionsBuilder.Options);
    }
}