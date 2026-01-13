using MerchantReport.Domain.Repositories;
using MerchantReport.Domain.Services;
using MerchantReport.Infrastructure.Configurations;
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
        services.AddDbContext<MerchantReportDbContext>(options =>
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(MerchantReportDbContext).Assembly.FullName)));

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