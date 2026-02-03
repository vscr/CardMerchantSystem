using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Transaction.Domain.Repositories;
using Transaction.Domain.Services;
using Transaction.Infrastructure.Configurations;
using Transaction.Infrastructure.Dapper;
using Transaction.Infrastructure.Persistence;
using Transaction.Infrastructure.Repositories;
using Transaction.Infrastructure.Services;

namespace Transaction.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTransactionInfrastructure(
        this IServiceCollection services,
        string connectionString,
        string redisConnectionString)
    {
        // DbContext (Write operations)
        services.AddDbContext<TransactionDbContext>(options =>
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(TransactionDbContext).Assembly.FullName)));

        // Redis
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConnectionString));

        // === EF Core Repositories (Write + Complex Queries) ===
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        // === Dapper Repositories (High-Performance Read) ===
        services.AddSingleton<IDapperContext, DapperContext>();
        services.AddScoped<ITransactionReadRepository, TransactionReadRepository>();
        services.AddScoped<ITransactionReportRepository, TransactionReportRepository>();

        // Services
        services.AddScoped<ILimitService, LimitService>();
        services.AddScoped<IFraudService, FraudService>();

        return services;
    }
}