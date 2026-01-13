using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Statement.Domain.Repositories;
using Statement.Domain.Services;
using Statement.Infrastructure.Persistence;
using Statement.Infrastructure.Repositories;
using Statement.Infrastructure.Services;

namespace Statement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddStatementInfrastructure(this IServiceCollection services, string connectionString)
    {
        // DbContext
        services.AddDbContext<StatementDbContext>(options =>
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(StatementDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<ICardStatementRepository, CardStatementRepository>();
        services.AddScoped<IStatementPeriodConfigRepository, StatementPeriodConfigRepository>();
        services.AddScoped<IStatementNotificationRepository, StatementNotificationRepository>();

        // Services
        services.AddScoped<IStatementPdfService, StatementPdfService>();
        services.AddScoped<IStatementNotificationService, StatementNotificationService>();

        return services;
    }
}