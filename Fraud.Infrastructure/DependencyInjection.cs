using CardMerchantSystem.Shared.Audit.Interceptors;
using Fraud.Application.Services;
using Fraud.Domain.Repositories;
using Fraud.Infrastructure.Persistence;
using Fraud.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fraud.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFraudInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        // DbContext
        services.AddDbContext<FraudDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(FraudDbContext).Assembly.FullName));

            var auditInterceptor = sp.GetService<AuditSaveChangesInterceptor>();
            if (auditInterceptor != null)
            {
                options.AddInterceptors(auditInterceptor);
            }
        });

        // Repositories
        services.AddScoped<IFraudRuleRepository, FraudRuleRepository>();
        services.AddScoped<IFraudScenarioRepository, FraudScenarioRepository>();
        services.AddScoped<IHitScenarioRepository, HitScenarioRepository>();
        services.AddScoped<IFraudAlertRepository, FraudAlertRepository>();
        services.AddScoped<IFraudActionRepository, FraudActionRepository>();
        services.AddScoped<ICardFraudProfileRepository, CardFraudProfileRepository>();
        services.AddScoped<IFraudBlacklistRepository, FraudBlacklistRepository>();

        // Services
        services.AddScoped<IFraudEngine, FraudEngine>();
        services.AddScoped<IRuleEvaluator, RuleEvaluator>();
        services.AddScoped<IBlacklistService, BlacklistService>();

        return services;
    }
}