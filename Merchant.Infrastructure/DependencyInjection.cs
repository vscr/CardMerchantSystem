
using CardMerchantSystem.Shared.Data;
using CardMerchantSystem.Shared.Data.Extensions;
using Merchant.Domain.Repositories;
using Merchant.Infrastructure.Data.Dapper;
using Merchant.Infrastructure.Persistence;
using Merchant.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Merchant.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMerchantInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database options
        var databaseOptions = configuration
            .GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>() ?? new DatabaseOptions();

        var connectionString = databaseOptions.GetConnectionString();
        var provider = databaseOptions.Provider;

        // DbContext
        services.AddDbContext<MerchantDbContext>(options =>
        {
            options.ConfigureDatabase(provider, connectionString);
        });

        // Dapper Context
        services.AddScoped<CardMerchantSystem.Shared.Data.Dapper.IDapperContext>(sp =>
        {
            return new MerchantDapperContext(configuration);
        });

        // Repositories
        services.AddScoped<IMerchantRepository, MerchantRepository>();
        services.AddScoped<IMerchantDapperRepository, MerchantDapperRepository>();

        return services;
    }
}