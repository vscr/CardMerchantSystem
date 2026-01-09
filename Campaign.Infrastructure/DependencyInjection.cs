using Campaign.Domain.Repositories;
using Campaign.Infrastructure.Configurations;
using Campaign.Infrastructure.Persistence;
using Campaign.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Campaign.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCampaignInfrastructure(this IServiceCollection services, string connectionString)
    {
        // DbContext
        services.AddDbContext<CampaignDbContext>(options =>
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(CampaignDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<ICampaignRepository, CampaignRepository>();

        return services;
    }
}