using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Merchant.Infrastructure.Persistence;

public class MerchantDbContextFactory : IDesignTimeDbContextFactory<MerchantDbContext>
{
    public MerchantDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../CardMerchantSystem.API"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration["Database:SqlServerConnection"];

        var optionsBuilder = new DbContextOptionsBuilder<MerchantDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new MerchantDbContext(optionsBuilder.Options, null);
    }
}