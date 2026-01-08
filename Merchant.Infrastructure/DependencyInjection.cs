using Merchant.Domain.Repositories;
using Merchant.Infrastructure.Configurations;
using Merchant.Infrastructure.Persistence;
using Merchant.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Merchant.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMerchantInfrastructure(this IServiceCollection services, string connectionString)
    {
        // DbContext
        services.AddDbContext<MerchantDbContext>(options =>
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(MerchantDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<IMerchantRepository, MerchantRepository>();

        return services;
    }
}