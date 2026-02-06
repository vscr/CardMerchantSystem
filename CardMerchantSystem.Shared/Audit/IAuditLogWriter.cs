using CardMerchantSystem.Shared.Audit.Entities;

namespace CardMerchantSystem.Shared.Audit;

/// <summary>
/// Audit loglarını veritabanına yazan servis.
/// DbContext'ten bağımsız çalışır, Dapper ile doğrudan SQL kullanır.
/// </summary>
public interface IAuditLogWriter
{
    /// <summary>
    /// Audit log kaydeder
    /// </summary>
    Task WriteAsync(AuditLog auditLog, CancellationToken cancellationToken = default);

    /// <summary>
    /// Birden fazla audit log kaydeder (batch)
    /// </summary>
    Task WriteBatchAsync(IEnumerable<AuditLog> auditLogs, CancellationToken cancellationToken = default);
}