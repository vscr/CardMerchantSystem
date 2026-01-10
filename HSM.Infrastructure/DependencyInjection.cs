using HSM.Domain.Repositories;
using HSM.Domain.Services;
using HSM.Infrastructure.Configurations;
using HSM.Infrastructure.Persistence;
using HSM.Infrastructure.Repositories;
using HSM.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HSM.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddHSMInfrastructure(this IServiceCollection services, string connectionString)
    {
        // DbContext
        services.AddDbContext<HSMDbContext>(options =>
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(HSMDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<IHSMDeviceRepository, HSMDeviceRepository>();
        services.AddScoped<IHSMKeyRepository, HSMKeyRepository>();
        services.AddScoped<IHSMCommandLogRepository, HSMCommandLogRepository>();

        // HSM Service (Simulator)
        services.AddScoped<IHSMService, HSMSimulatorService>();

        return services;
    }
}