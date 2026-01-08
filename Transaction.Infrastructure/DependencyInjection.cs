using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Transaction.Domain.Repositories;
using Transaction.Domain.Services;
using Transaction.Infrastructure.Configurations;
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
        // DbContext
        services.AddDbContext<TransactionDbContext>(options =>
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(TransactionDbContext).Assembly.FullName)));

        // Redis
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConnectionString));

        // Repositories
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        // Services
        services.AddScoped<ILimitService, LimitService>();
        services.AddScoped<IFraudService, FraudService>();

        return services;
    }
}