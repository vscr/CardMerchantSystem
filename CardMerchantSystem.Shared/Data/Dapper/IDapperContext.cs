// CardMerchantSystem.Shared/Data/Dapper/IDapperContext.cs

using System.Data;

namespace CardMerchantSystem.Shared.Data.Dapper;

/// <summary>
/// Dapper için veritabanı bağlantı context'i
/// </summary>
public interface IDapperContext
{
    /// <summary>
    /// Veritabanı bağlantısı oluşturur
    /// </summary>
    IDbConnection CreateConnection();

    /// <summary>
    /// Yeni bir transaction başlatır
    /// </summary>
    IDbTransaction BeginTransaction();
}