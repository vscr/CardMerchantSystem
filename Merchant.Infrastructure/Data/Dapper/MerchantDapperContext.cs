
using CardMerchantSystem.Shared.Data;
using CardMerchantSystem.Shared.Data.Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace Merchant.Infrastructure.Data.Dapper;

public class MerchantDapperContext : IDapperContext
{
    private readonly string _connectionString;
    private readonly DatabaseProvider _provider;

    public MerchantDapperContext(IConfiguration configuration)
    {
        var databaseOptions = configuration
            .GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>() ?? new DatabaseOptions();

        _connectionString = databaseOptions.GetConnectionString();
        _provider = databaseOptions.Provider;
    }

    public IDbConnection CreateConnection()
    {
        return _provider switch
        {
            DatabaseProvider.SqlServer => new SqlConnection(_connectionString),
            DatabaseProvider.PostgreSql => new NpgsqlConnection(_connectionString),
            _ => throw new InvalidOperationException($"Unsupported database provider: {_provider}")
        };
    }

    public IDbTransaction BeginTransaction()
    {
        var connection = CreateConnection();
        connection.Open();
        return connection.BeginTransaction();
    }
}