// CardMerchantSystem.Shared/Data/Dapper/IDapperContext.cs

using System.Data;

namespace CardMerchantSystem.Shared.Data.Dapper;

/// <summary>
/// Dapper için veritabanı bağlantı context'i
/// </summary>
public interface IDapperContext
{
    /// <summary>
    /// SQL Server bağlantısı oluşturur
    /// </summary>
    IDbConnection CreateConnection();

    /// <summary>
    /// Transaction başlatır
    /// </summary>
    IDbTransaction BeginTransaction();

    /// <summary>
    /// Transaction başlatır (bağlantı ile)
    /// </summary>
    IDbTransaction BeginTransaction(IDbConnection connection);
}