// Merchant.Infrastructure/DependencyInjection.cs

using CardMerchantSystem.Shared.Data.Dapper;
using Merchant.Domain.Repositories;
using Merchant.Infrastructure.Data;
using Merchant.Infrastructure.Data.Dapper;
using Merchant.Infrastructure.Persistence;
using Merchant.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Merchant.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMerchantInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        // EF DbContext
        services.AddDbContext<MerchantDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Dapper Context
        services.AddScoped<IDapperContext>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            return new MerchantDapperContext(configuration);
        });

        // EF Repositories
        services.AddScoped<IMerchantRepository, MerchantRepository>();

        // Dapper Repositories
        services.AddScoped<IMerchantDapperRepository, MerchantDapperRepository>();

        return services;
    }
}