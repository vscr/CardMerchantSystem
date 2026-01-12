using Fee.Domain.Repositories;
using Fee.Domain.Services;
using Fee.Infrastructure.Persistence;
using Fee.Infrastructure.Repositories;
using Fee.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fee.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFeeInfrastructure(this IServiceCollection services, string connectionString)
    {
        // DbContext
        services.AddDbContext<FeeDbContext>(options =>
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(FeeDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<ITariffRepository, TariffRepository>();
        services.AddScoped<IMerchantTariffRepository, MerchantTariffRepository>();
        services.AddScoped<IFeeAccrualRepository, FeeAccrualRepository>();
        services.AddScoped<IMembershipFeeRepository, MembershipFeeRepository>();
        services.AddScoped<ICommissionBreakdownRepository, CommissionBreakdownRepository>();

        // Services
        services.AddScoped<IFeeCalculationService, FeeCalculationService>();

        return services;
    }
}