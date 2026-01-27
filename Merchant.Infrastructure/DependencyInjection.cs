using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Merchant.Infrastructure.Persistence;
using Merchant.Infrastructure.Repositories;
using Merchant.Domain.Repositories;
using CardMerchantSystem.Shared.Data;

namespace Merchant.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMerchantInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var databaseOptions = configuration
            .GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>() ?? new DatabaseOptions();

        if (databaseOptions.Provider == DatabaseProvider.PostgreSql)
        {
            services.AddDbContext<MerchantDbContext_Pg>(options =>
                options.UseNpgsql(databaseOptions.PostgreSqlConnection));

            services.AddScoped<MerchantDbContextBase>(sp =>
                sp.GetRequiredService<MerchantDbContext_Pg>());
        }
        else
        {
            services.AddDbContext<MerchantDbContext>(options =>
                options.UseSqlServer(databaseOptions.SqlServerConnection));

            services.AddScoped<MerchantDbContextBase>(sp =>
                sp.GetRequiredService<MerchantDbContext>());
        }

        // Repositories (Base context kullanır)
        services.AddScoped<IMerchantRepository, MerchantRepository>();

        return services;
    }
}