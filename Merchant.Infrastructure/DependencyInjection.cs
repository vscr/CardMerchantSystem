using System.Data;
using CardMerchantSystem.Shared.Audit.Interceptors;
using CardMerchantSystem.Shared.Data;
using CardMerchantSystem.Shared.Data.Extensions;
using Merchant.Domain.Repositories;
using Merchant.Infrastructure.Persistence;
using Merchant.Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Merchant.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMerchantInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var databaseOptions = configuration
            .GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>() ?? new DatabaseOptions();

        var connectionString = databaseOptions.GetConnectionString();
        var provider = databaseOptions.Provider;

        // EF Core DbContext with Audit Interceptor
        services.AddDbContext<MerchantDbContext>((sp, options) =>
        {
            options.ConfigureDatabase(provider, connectionString);

            // Audit interceptor ekle
            var auditInterceptor = sp.GetService<AuditSaveChangesInterceptor>();
            if (auditInterceptor != null)
            {
                options.AddInterceptors(auditInterceptor);
            }
        });

        // Base context alias — MerchantRepository bunu inject ediyor
        services.AddScoped<MerchantDbContextBase>(sp =>
            sp.GetRequiredService<MerchantDbContext>());

        // Dapper - IDbConnection
        services.AddScoped<IDbConnection>(sp =>
        {
            IDbConnection conn = provider switch
            {
                DatabaseProvider.SqlServer => new SqlConnection(connectionString),
                DatabaseProvider.PostgreSql => new NpgsqlConnection(connectionString),
                _ => throw new InvalidOperationException($"Unsupported provider: {provider}")
            };
            conn.Open();
            return conn;
        });

        // Repositories
        services.AddScoped<IMerchantRepository, MerchantRepository>();
        services.AddScoped<IMerchantDapperRepository, MerchantDapperRepository>();

        return services;
    }
}