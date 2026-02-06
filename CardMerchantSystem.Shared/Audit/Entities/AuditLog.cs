using CardMerchantSystem.Shared.Audit.Enums;

namespace CardMerchantSystem.Shared.Audit.Entities;

/// <summary>
/// Tüm entity değişikliklerinin audit kaydı.
/// Kim, ne zaman, hangi entity'de, ne değişiklik yaptı bilgilerini tutar.
/// </summary>
public class AuditLog
{
    public Guid Id { get; private set; }

    /// <summary>
    /// İşlemi yapan kullanıcı
    /// </summary>
    public string? UserId { get; private set; }

    /// <summary>
    /// İşlemi yapan kullanıcı adı
    /// </summary>
    public string? UserName { get; private set; }

    /// <summary>
    /// İşlem tipi (Insert/Update/Delete)
    /// </summary>
    public AuditActionType ActionType { get; private set; }

    /// <summary>
    /// Değişen entity'nin tipi (full name)
    /// </summary>
    public string EntityType { get; private set; } = null!;

    /// <summary>
    /// Değişen entity'nin kısa adı
    /// </summary>
    public string EntityName { get; private set; } = null!;

    /// <summary>
    /// Değişen entity'nin ID'si
    /// </summary>
    public string EntityId { get; private set; } = null!;

    /// <summary>
    /// Değişiklik öncesi değerler (JSON)
    /// Insert işleminde null
    /// </summary>
    public string? OldValues { get; private set; }

    /// <summary>
    /// Değişiklik sonrası değerler (JSON)
    /// Delete işleminde null
    /// </summary>
    public string? NewValues { get; private set; }

    /// <summary>
    /// Değişen property'ler (JSON array)
    /// </summary>
    public string? ChangedColumns { get; private set; }

    /// <summary>
    /// İşlemin yapıldığı tablo adı
    /// </summary>
    public string TableName { get; private set; } = null!;

    /// <summary>
    /// İşlem zamanı (UTC)
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// İşlemin yapıldığı IP adresi
    /// </summary>
    public string? IpAddress { get; private set; }

    /// <summary>
    /// Correlation ID (request tracking)
    /// </summary>
    public string? CorrelationId { get; private set; }

    /// <summary>
    /// Ek bilgiler (JSON)
    /// </summary>
    public string? AdditionalData { get; private set; }

    private AuditLog() { } // EF Core için

    public static AuditLog Create(
        string? userId,
        string? userName,
        AuditActionType actionType,
        string entityType,
        string entityName,
        string entityId,
        string tableName,
        string? oldValues,
        string? newValues,
        string? changedColumns,
        string? ipAddress = null,
        string? correlationId = null,
        string? additionalData = null)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            UserName = userName,
            ActionType = actionType,
            EntityType = entityType,
            EntityName = entityName,
            EntityId = entityId,
            TableName = tableName,
            OldValues = oldValues,
            NewValues = newValues,
            ChangedColumns = changedColumns,
            IpAddress = ipAddress,
            CorrelationId = correlationId,
            AdditionalData = additionalData,
            Timestamp = DateTime.UtcNow
        };
    }
}