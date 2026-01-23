// CardMerchantSystem.Shared/Data/Dapper/BaseDapperRepository.cs

using System.Data;
using Dapper;

namespace CardMerchantSystem.Shared.Data.Dapper;

/// <summary>
/// Tüm Dapper repository'leri için base class
/// </summary>
public abstract class BaseDapperRepository
{
    protected readonly IDapperContext _dapperContext;

    protected BaseDapperRepository(IDapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    /// <summary>
    /// Tek bir kayıt döner
    /// </summary>
    protected async Task<T?> QuerySingleOrDefaultAsync<T>(
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null)
    {
        using var connection = _dapperContext.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<T>(sql, param, transaction, commandTimeout);
    }

    /// <summary>
    /// Birden fazla kayıt döner
    /// </summary>
    protected async Task<IEnumerable<T>> QueryAsync<T>(
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null)
    {
        using var connection = _dapperContext.CreateConnection();
        return await connection.QueryAsync<T>(sql, param, transaction, commandTimeout);
    }

    /// <summary>
    /// İlk kaydı döner, bulamazsa exception fırlatır
    /// </summary>
    protected async Task<T> QueryFirstAsync<T>(
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null)
    {
        using var connection = _dapperContext.CreateConnection();
        return await connection.QueryFirstAsync<T>(sql, param, transaction, commandTimeout);
    }

    /// <summary>
    /// İlk kaydı döner veya null
    /// </summary>
    protected async Task<T?> QueryFirstOrDefaultAsync<T>(
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null)
    {
        using var connection = _dapperContext.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout);
    }

    /// <summary>
    /// Execute (INSERT, UPDATE, DELETE) - etkilenen satır sayısını döner
    /// </summary>
    protected async Task<int> ExecuteAsync(
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null)
    {
        using var connection = _dapperContext.CreateConnection();
        return await connection.ExecuteAsync(sql, param, transaction, commandTimeout);
    }

    /// <summary>
    /// Scalar değer döner (COUNT, SUM, vb.)
    /// </summary>
    protected async Task<T> ExecuteScalarAsync<T>(
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null)
    {
        using var connection = _dapperContext.CreateConnection();
        var result = await connection.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout);
        return result;
    }

    /// <summary>
    /// Multiple result sets - birden fazla sorgu sonucu
    /// </summary>
    protected async Task<SqlMapper.GridReader> QueryMultipleAsync(
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null)
    {
        var connection = _dapperContext.CreateConnection();
        return await connection.QueryMultipleAsync(sql, param, transaction, commandTimeout);
    }

    /// <summary>
    /// Stored Procedure çalıştırır
    /// </summary>
    protected async Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(
        string storedProcedureName,
        object? param = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null)
    {
        using var connection = _dapperContext.CreateConnection();
        return await connection.QueryAsync<T>(
            storedProcedureName,
            param,
            transaction,
            commandTimeout,
            CommandType.StoredProcedure);
    }

    /// <summary>
    /// Bulk insert işlemi
    /// </summary>
    protected async Task<int> BulkInsertAsync<T>(
        string sql,
        IEnumerable<T> entities,
        IDbTransaction? transaction = null,
        int? commandTimeout = null)
    {
        using var connection = _dapperContext.CreateConnection();
        return await connection.ExecuteAsync(sql, entities, transaction, commandTimeout);
    }

    /// <summary>
    /// Sayfalama desteği ile sorgu
    /// </summary>
    protected async Task<(IEnumerable<T> Data, int TotalCount)> QueryPagedAsync<T>(
        string sql,
        string countSql,
        object? param = null,
        int pageNumber = 1,
        int pageSize = 10,
        IDbTransaction? transaction = null,
        int? commandTimeout = null)
    {
        using var connection = _dapperContext.CreateConnection();

        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, param, transaction, commandTimeout);

        var pagedParam = new DynamicParameters(param);
        pagedParam.Add("@Offset", (pageNumber - 1) * pageSize);
        pagedParam.Add("@PageSize", pageSize);

        var data = await connection.QueryAsync<T>(sql, pagedParam, transaction, commandTimeout);

        return (data, totalCount);
    }
}