using EarlyBlockResolution.Domain.Repositories;
using EarlyBlockResolution.Infrastructure.Persistence;
using EarlyBlockResolution.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EarlyBlockResolution.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddEarlyBlockResolutionInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<EarlyBlockResolutionDbContext>(options =>
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(EarlyBlockResolutionDbContext).Assembly.FullName)));

        services.AddScoped<IBlockRuleRepository, BlockRuleRepository>();
        services.AddScoped<IFraudAlertRepository, FraudAlertRepository>();
        services.AddScoped<ICardBlockRepository, CardBlockRepository>();

        return services;
    }
}