using CardMerchantSystem.Shared.Audit.Interceptors;
using MerchantReport.Domain.Repositories;
using MerchantReport.Domain.Services;
using MerchantReport.Infrastructure.Persistence;
using MerchantReport.Infrastructure.Repositories;
using MerchantReport.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MerchantReport.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMerchantReportInfrastructure(this IServiceCollection services, string connectionString)
    {
        // DbContext
        services.AddDbContext<MerchantReportDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(MerchantReportDbContext).Assembly.FullName));

            // Audit interceptor ekle
            var auditInterceptor = sp.GetService<AuditSaveChangesInterceptor>();
            if (auditInterceptor != null)
            {
                options.AddInterceptors(auditInterceptor);
            }
        });

        // Repositories
        services.AddScoped<IMerchantReportConfigRepository, MerchantReportConfigRepository>();
        services.AddScoped<IReportRequestRepository, ReportRequestRepository>();
        services.AddScoped<IMerchantStatementRepository, MerchantStatementRepository>();

        // Services
        services.AddScoped<IReportGeneratorService, ReportGeneratorService>();
        services.AddScoped<IReportDeliveryService, ReportDeliveryService>();

        return services;
    }
}