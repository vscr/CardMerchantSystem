using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Transaction.Infrastructure.Dapper;

/// <summary>
/// Dapper için DbConnection factory
/// </summary>
public interface IDapperContext
{
    IDbConnection CreateConnection();
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}

public class DapperContext : IDapperContext
{
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        // Önce TransactionDb'yi dene, yoksa DefaultConnection'ı kullan
        _connectionString = configuration.GetConnectionString("TransactionDb")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? configuration["Database:SqlServerConnection"]
            ?? throw new InvalidOperationException("No valid connection string found. Please configure 'ConnectionStrings:DefaultConnection' or 'Database:SqlServerConnection'");
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}