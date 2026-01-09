using BKM.Domain.Repositories;
using BKM.Infrastructure.Configurations;
using BKM.Infrastructure.Persistence;
using BKM.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BKM.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBKMInfrastructure(this IServiceCollection services, string connectionString)
    {
        // DbContext
        services.AddDbContext<BKMDbContext>(options =>
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(BKMDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<ISwitchMessageRepository, SwitchMessageRepository>();
        services.AddScoped<IClearingRepository, ClearingRepository>();
        services.AddScoped<ISettlementRepository, SettlementRepository>();
        services.AddScoped<IBINTableRepository, BINTableRepository>();

        return services;
    }
}