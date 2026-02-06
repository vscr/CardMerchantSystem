using System.Data;
using CardMerchantSystem.Shared.Audit.Entities;
using CardMerchantSystem.Shared.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CardMerchantSystem.Shared.Audit;

/// <summary>
/// Dapper kullanarak audit logları doğrudan SQL ile yazan servis.
/// DbContext'ten bağımsız çalışır.
/// </summary>
public class AuditLogWriter : IAuditLogWriter
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuditLogWriter> _logger;
    private readonly string _connectionString;
    private readonly DatabaseProvider _provider;
    private readonly string _tableName;

    public AuditLogWriter(
        IConfiguration configuration,
        ILogger<AuditLogWriter> logger)
    {
        _configuration = configuration;
        _logger = logger;

        var databaseOptions = configuration
            .GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>() ?? new DatabaseOptions();

        _connectionString = databaseOptions.GetConnectionString();
        _provider = databaseOptions.Provider;

        // Schema ve tablo adı
        _tableName = _provider == DatabaseProvider.PostgreSql
            ? "audit.\"AuditLogs\""
            : "[audit].[AuditLogs]";
    }

    public async Task WriteAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = CreateConnection();
            await connection.ExecuteAsync(GetInsertSql(), MapToParameters(auditLog));
        }
        catch (Exception ex)
        {
            // Audit hatası ana işlemi engellememelidir
            _logger.LogError(ex, "Audit log yazma hatası: {EntityName} {EntityId}",
                auditLog.EntityName, auditLog.EntityId);
        }
    }

    public async Task WriteBatchAsync(IEnumerable<AuditLog> auditLogs, CancellationToken cancellationToken = default)
    {
        if (!auditLogs.Any()) return;

        try
        {
            using var connection = CreateConnection();
            var sql = GetInsertSql();

            foreach (var auditLog in auditLogs)
            {
                await connection.ExecuteAsync(sql, MapToParameters(auditLog));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Batch audit log yazma hatası");
        }
    }

    private IDbConnection CreateConnection()
    {
        IDbConnection connection = _provider switch
        {
            DatabaseProvider.SqlServer => new Microsoft.Data.SqlClient.SqlConnection(_connectionString),
            DatabaseProvider.PostgreSql => new Npgsql.NpgsqlConnection(_connectionString),
            _ => throw new InvalidOperationException($"Unsupported provider: {_provider}")
        };

        connection.Open();
        return connection;
    }

    private string GetInsertSql()
    {
        return $@"
            INSERT INTO {_tableName} (
                Id, UserId, UserName, ActionType, EntityType, EntityName, 
                EntityId, OldValues, NewValues, ChangedColumns, TableName, 
                Timestamp, IpAddress, CorrelationId, AdditionalData
            ) VALUES (
                @Id, @UserId, @UserName, @ActionType, @EntityType, @EntityName,
                @EntityId, @OldValues, @NewValues, @ChangedColumns, @TableName,
                @Timestamp, @IpAddress, @CorrelationId, @AdditionalData
            )";
    }

    private static object MapToParameters(AuditLog log)
    {
        return new
        {
            log.Id,
            log.UserId,
            log.UserName,
            ActionType = (int)log.ActionType,
            log.EntityType,
            log.EntityName,
            log.EntityId,
            log.OldValues,
            log.NewValues,
            log.ChangedColumns,
            log.TableName,
            log.Timestamp,
            log.IpAddress,
            log.CorrelationId,
            log.AdditionalData
        };
    }
}