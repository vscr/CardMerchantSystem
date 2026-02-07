using CardMerchantSystem.Shared.Audit.Interceptors;
using Dispute.Domain.Repositories;
using Dispute.Infrastructure.Persistence;
using Dispute.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dispute.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDisputeInfrastructure(this IServiceCollection services, string connectionString)
    {
        // DbContext
        services.AddDbContext<DisputeDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(DisputeDbContext).Assembly.FullName));

            // Audit interceptor ekle
            var auditInterceptor = sp.GetService<AuditSaveChangesInterceptor>();
            if (auditInterceptor != null)
            {
                options.AddInterceptors(auditInterceptor);
            }
        });

        // Repositories
        services.AddScoped<IDisputeRepository, DisputeRepository>();

        return services;
    }
}