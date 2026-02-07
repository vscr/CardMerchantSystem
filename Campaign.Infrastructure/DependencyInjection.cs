using Campaign.Domain.Repositories;
using Campaign.Infrastructure.Configurations;
using Campaign.Infrastructure.Persistence;
using Campaign.Infrastructure.Repositories;
using CardMerchantSystem.Shared.Audit.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Campaign.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCampaignInfrastructure(this IServiceCollection services, string connectionString)
    {
        // DbContext
        services.AddDbContext<CampaignDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(CampaignDbContext).Assembly.FullName));

            // Audit interceptor ekle
            var auditInterceptor = sp.GetService<AuditSaveChangesInterceptor>();
            if (auditInterceptor != null)
            {
                options.AddInterceptors(auditInterceptor);
            }
        });

        // Repositories
        services.AddScoped<ICampaignRepository, CampaignRepository>();

        return services;
    }
}