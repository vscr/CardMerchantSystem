using System.Data;
using System.Text.Json;
using CardMerchantSystem.Shared.Audit.DTOs;
using CardMerchantSystem.Shared.Audit.Entities;
using CardMerchantSystem.Shared.Audit.Enums;
using CardMerchantSystem.Shared.Data;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace CardMerchantSystem.Shared.Audit.Services;

/// <summary>
/// Audit log sorgulama servisi implementasyonu (Dapper).
/// </summary>
public class AuditService : IAuditService
{
    private readonly string _connectionString;
    private readonly DatabaseProvider _provider;
    private readonly string _tableName;

    public AuditService(IConfiguration configuration)
    {
        var databaseOptions = configuration
            .GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>() ?? new DatabaseOptions();

        _connectionString = databaseOptions.GetConnectionString();
        _provider = databaseOptions.Provider;

        _tableName = _provider == DatabaseProvider.PostgreSql
            ? "audit.\"AuditLogs\""
            : "[audit].[AuditLogs]";
    }

    public async Task<IReadOnlyList<AuditLogDto>> GetEntityHistoryAsync(
        string entityName,
        string entityId,
        CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        var sql = $@"
            SELECT * FROM {_tableName} 
            WHERE EntityName = @EntityName AND EntityId = @EntityId 
            ORDER BY Timestamp DESC";

        var logs = await connection.QueryAsync<AuditLogRecord>(sql, new { EntityName = entityName, EntityId = entityId });
        return logs.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<AuditLogDto>> GetUserActionsAsync(
        string userId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        var sql = $@"
            SELECT TOP 1000 * FROM {_tableName} 
            WHERE UserId = @UserId";

        if (from.HasValue)
            sql += " AND Timestamp >= @From";
        if (to.HasValue)
            sql += " AND Timestamp <= @To";

        sql += " ORDER BY Timestamp DESC";

        // PostgreSQL için TOP yerine LIMIT
        if (_provider == DatabaseProvider.PostgreSql)
            sql = sql.Replace("TOP 1000 *", "*").Replace("ORDER BY Timestamp DESC", "ORDER BY Timestamp DESC LIMIT 1000");

        var logs = await connection.QueryAsync<AuditLogRecord>(sql, new { UserId = userId, From = from, To = to });
        return logs.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<AuditLogDto>> GetByDateRangeAsync(
        DateTime from,
        DateTime to,
        string? entityName = null,
        AuditActionType? actionType = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        var sql = $@"
            SELECT TOP 1000 * FROM {_tableName} 
            WHERE Timestamp >= @From AND Timestamp <= @To";

        if (!string.IsNullOrEmpty(entityName))
            sql += " AND EntityName = @EntityName";
        if (actionType.HasValue)
            sql += " AND ActionType = @ActionType";

        sql += " ORDER BY Timestamp DESC";

        if (_provider == DatabaseProvider.PostgreSql)
            sql = sql.Replace("TOP 1000 *", "*").Replace("ORDER BY Timestamp DESC", "ORDER BY Timestamp DESC LIMIT 1000");

        var logs = await connection.QueryAsync<AuditLogRecord>(sql, new
        {
            From = from,
            To = to,
            EntityName = entityName,
            ActionType = (int?)actionType
        });
        return logs.Select(MapToDto).ToList();
    }

    public async Task<PagedAuditResult> GetPagedAsync(
        AuditLogFilter filter,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        var whereClause = "WHERE 1=1";
        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(filter.EntityName))
        {
            whereClause += " AND EntityName = @EntityName";
            parameters.Add("EntityName", filter.EntityName);
        }

        if (!string.IsNullOrEmpty(filter.EntityId))
        {
            whereClause += " AND EntityId = @EntityId";
            parameters.Add("EntityId", filter.EntityId);
        }

        if (!string.IsNullOrEmpty(filter.UserId))
        {
            whereClause += " AND UserId = @UserId";
            parameters.Add("UserId", filter.UserId);
        }

        if (!string.IsNullOrEmpty(filter.UserName))
        {
            whereClause += " AND UserName LIKE @UserName";
            parameters.Add("UserName", $"%{filter.UserName}%");
        }

        if (filter.ActionType.HasValue)
        {
            whereClause += " AND ActionType = @ActionType";
            parameters.Add("ActionType", (int)filter.ActionType.Value);
        }

        if (filter.FromDate.HasValue)
        {
            whereClause += " AND Timestamp >= @FromDate";
            parameters.Add("FromDate", filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            whereClause += " AND Timestamp <= @ToDate";
            parameters.Add("ToDate", filter.ToDate.Value);
        }

        if (!string.IsNullOrEmpty(filter.CorrelationId))
        {
            whereClause += " AND CorrelationId = @CorrelationId";
            parameters.Add("CorrelationId", filter.CorrelationId);
        }

        // Count query
        var countSql = $"SELECT COUNT(*) FROM {_tableName} {whereClause}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        // Data query with pagination
        var offset = (page - 1) * pageSize;
        string dataSql;

        if (_provider == DatabaseProvider.PostgreSql)
        {
            dataSql = $@"
                SELECT * FROM {_tableName} 
                {whereClause} 
                ORDER BY Timestamp DESC 
                LIMIT @PageSize OFFSET @Offset";
        }
        else
        {
            dataSql = $@"
                SELECT * FROM {_tableName} 
                {whereClause} 
                ORDER BY Timestamp DESC 
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        }

        parameters.Add("PageSize", pageSize);
        parameters.Add("Offset", offset);

        var logs = await connection.QueryAsync<AuditLogRecord>(dataSql, parameters);

        return new PagedAuditResult
        {
            Items = logs.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<AuditLogDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        var sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";
        var log = await connection.QueryFirstOrDefaultAsync<AuditLogRecord>(sql, new { Id = id });

        return log == null ? null : MapToDto(log);
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

    private static AuditLogDto MapToDto(AuditLogRecord log)
    {
        return new AuditLogDto
        {
            Id = log.Id,
            UserId = log.UserId,
            UserName = log.UserName,
            ActionType = (AuditActionType)log.ActionType,
            EntityType = log.EntityType,
            EntityName = log.EntityName,
            EntityId = log.EntityId,
            OldValues = log.OldValues,
            NewValues = log.NewValues,
            ChangedColumns = log.ChangedColumns,
            TableName = log.TableName,
            Timestamp = log.Timestamp,
            IpAddress = log.IpAddress,
            CorrelationId = log.CorrelationId,
            ChangedColumnsList = TryDeserializeList(log.ChangedColumns),
            OldValuesDictionary = TryDeserializeDictionary(log.OldValues),
            NewValuesDictionary = TryDeserializeDictionary(log.NewValues)
        };
    }

    private static List<string>? TryDeserializeList(string? json)
    {
        if (string.IsNullOrEmpty(json)) return null;
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json);
        }
        catch
        {
            return null;
        }
    }

    private static Dictionary<string, object?>? TryDeserializeDictionary(string? json)
    {
        if (string.IsNullOrEmpty(json)) return null;
        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object?>>(json);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Dapper mapping için internal record
    /// </summary>
    private record AuditLogRecord
    {
        public Guid Id { get; init; }
        public string? UserId { get; init; }
        public string? UserName { get; init; }
        public int ActionType { get; init; }
        public string EntityType { get; init; } = null!;
        public string EntityName { get; init; } = null!;
        public string EntityId { get; init; } = null!;
        public string? OldValues { get; init; }
        public string? NewValues { get; init; }
        public string? ChangedColumns { get; init; }
        public string TableName { get; init; } = null!;
        public DateTime Timestamp { get; init; }
        public string? IpAddress { get; init; }
        public string? CorrelationId { get; init; }
        public string? AdditionalData { get; init; }
    }
}