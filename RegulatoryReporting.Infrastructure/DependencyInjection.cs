using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RegulatoryReporting.Domain.Repositories;
using RegulatoryReporting.Infrastructure.Persistence;
using RegulatoryReporting.Infrastructure.Repositories;

namespace RegulatoryReporting.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddRegulatoryReportingInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<RegulatoryReportingDbContext>(options =>
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(RegulatoryReportingDbContext).Assembly.FullName)));

        services.AddScoped<IReportDefinitionRepository, ReportDefinitionRepository>();
        services.AddScoped<IReportScheduleRepository, ReportScheduleRepository>();
        services.AddScoped<IGeneratedReportRepository, GeneratedReportRepository>();

        return services;
    }
}