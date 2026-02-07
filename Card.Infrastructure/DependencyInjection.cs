using Card.Domain.Repositories;
using Card.Infrastructure.Persistence;
using Card.Infrastructure.Repositories;
using CardMerchantSystem.Shared.Audit.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Card.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCardInfrastructure(this IServiceCollection services, string connectionString)
    {
        // DbContext
        services.AddDbContext<CardDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(CardDbContext).Assembly.FullName));

            // Audit interceptor ekle
            var auditInterceptor = sp.GetService<AuditSaveChangesInterceptor>();
            if (auditInterceptor != null)
            {
                options.AddInterceptors(auditInterceptor);
            }
        });

        // Repositories
        services.AddScoped<ICardApplicationRepository, CardApplicationRepository>();

        return services;
    }
}