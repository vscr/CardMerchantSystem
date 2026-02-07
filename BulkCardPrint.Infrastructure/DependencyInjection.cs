using BulkCardPrint.Domain.Repositories;
using BulkCardPrint.Infrastructure.Persistence;
using BulkCardPrint.Infrastructure.Repositories;
using CardMerchantSystem.Shared.Audit.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BulkCardPrint.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBulkCardPrintInfrastructure(this IServiceCollection services, string connectionString)
    {
        // DbContext
        services.AddDbContext<BulkCardPrintDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(BulkCardPrintDbContext).Assembly.FullName));

            // Audit interceptor ekle
            var auditInterceptor = sp.GetService<AuditSaveChangesInterceptor>();
            if (auditInterceptor != null)
            {
                options.AddInterceptors(auditInterceptor);
            }
        });

        // Repositories
        services.AddScoped<IPrintVendorRepository, PrintVendorRepository>();
        services.AddScoped<IPrintBatchRepository, PrintBatchRepository>();

        return services;
    }
}