// CardMerchantSystem.Shared/Data/DatabaseOptions.cs

namespace CardMerchantSystem.Shared.Data;

/// <summary>
/// Veritabanı yapılandırma ayarları
/// </summary>
public class DatabaseOptions
{
    public const string SectionName = "Database";

    /// <summary>
    /// Kullanılacak veritabanı sağlayıcısı (SqlServer veya PostgreSql)
    /// </summary>
    public DatabaseProvider Provider { get; set; } = DatabaseProvider.SqlServer;

    /// <summary>
    /// SQL Server connection string
    /// </summary>
    public string SqlServerConnection { get; set; } = string.Empty;

    /// <summary>
    /// PostgreSQL connection string
    /// </summary>
    public string PostgreSqlConnection { get; set; } = string.Empty;

    /// <summary>
    /// Aktif connection string'i döner
    /// </summary>
    public string GetConnectionString()
    {
        return Provider switch
        {
            DatabaseProvider.SqlServer => SqlServerConnection,
            DatabaseProvider.PostgreSql => PostgreSqlConnection,
            _ => throw new InvalidOperationException($"Unsupported database provider: {Provider}")
        };
    }
}