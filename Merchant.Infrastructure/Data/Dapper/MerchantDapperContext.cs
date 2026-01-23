using System.Data;
using System.Data.SqlClient;
using CardMerchantSystem.Shared.Data.Dapper;
using Microsoft.Extensions.Configuration;

namespace Merchant.Infrastructure.Data.Dapper;

public class MerchantDapperContext : IDapperContext
{
    private readonly string _connectionString;

    // Constructor - IConfiguration ile
    public MerchantDapperContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("MerchantDb connection string not found.");
    }

    // Alternatif constructor - direkt connectionString ile
    public MerchantDapperContext(string connectionString)
    {
        _connectionString = connectionString
            ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    public IDbTransaction BeginTransaction()
    {
        var connection = CreateConnection();
        connection.Open();
        return connection.BeginTransaction();
    }

    public IDbTransaction BeginTransaction(IDbConnection connection)
    {
        if (connection.State != ConnectionState.Open)
            connection.Open();

        return connection.BeginTransaction();
    }
}