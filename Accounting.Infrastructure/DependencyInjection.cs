using Accounting.Domain.Repositories;
using Accounting.Domain.Services;
using Accounting.Infrastructure.Persistence;
using Accounting.Infrastructure.Repositories;
using Accounting.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Accounting.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountingInfrastructure(this IServiceCollection services, string connectionString)
    {
        // DbContext
        services.AddDbContext<AccountingDbContext>(options =>
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(AccountingDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<IChartOfAccountRepository, ChartOfAccountRepository>();
        services.AddScoped<IAccountingPeriodRepository, AccountingPeriodRepository>();
        services.AddScoped<IJournalEntryRepository, JournalEntryRepository>();
        services.AddScoped<IAccountBalanceRepository, AccountBalanceRepository>();

        // Services
        services.AddScoped<ITrialBalanceService, TrialBalanceService>();
        services.AddScoped<IAccountingService, AccountingService>();

        return services;
    }
}