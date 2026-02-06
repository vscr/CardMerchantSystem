using CardMerchantSystem.Shared.Audit.Entities;
using CardMerchantSystem.Shared.Audit.Enums;
using CardMerchantSystem.Shared.Kernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace CardMerchantSystem.Shared.Audit;

/// <summary>
/// SaveChanges sırasında değişiklikleri capture eden helper class.
/// EntityEntry'den AuditLog'a dönüşüm yapar.
/// </summary>
public class AuditEntry
{
    public EntityEntry Entry { get; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? IpAddress { get; set; }
    public string? CorrelationId { get; set; }
    public string TableName { get; set; } = null!;
    public string EntityType { get; set; } = null!;
    public string EntityName { get; set; } = null!;
    public AuditActionType ActionType { get; set; }
    public Dictionary<string, object?> OldValues { get; } = new();
    public Dictionary<string, object?> NewValues { get; } = new();
    public List<string> ChangedColumns { get; } = new();

    /// <summary>
    /// Temporary properties (henüz DB'den ID almamış entity'ler için)
    /// </summary>
    public List<PropertyEntry> TemporaryProperties { get; } = new();

    /// <summary>
    /// Insert işlemlerinde ID henüz generate edilmemiş olabilir
    /// </summary>
    public bool HasTemporaryProperties => TemporaryProperties.Any();

    public AuditEntry(EntityEntry entry)
    {
        Entry = entry;
        var type = entry.Entity.GetType();
        EntityType = type.FullName ?? type.Name;
        EntityName = type.Name;
        TableName = entry.Metadata.GetTableName() ?? type.Name;
    }

    /// <summary>
    /// AuditEntry'den AuditLog entity'si oluşturur
    /// </summary>
    public AuditLog ToAuditLog()
    {
        // Entity ID'yi al
        var primaryKey = Entry.Properties
            .FirstOrDefault(p => p.Metadata.IsPrimaryKey());

        var entityId = primaryKey?.CurrentValue?.ToString() ?? "Unknown";

        return AuditLog.Create(
            userId: UserId,
            userName: UserName,
            actionType: ActionType,
            entityType: EntityType,
            entityName: EntityName,
            entityId: entityId,
            tableName: TableName,
            oldValues: OldValues.Count > 0 ? SerializeSafe(OldValues) : null,
            newValues: NewValues.Count > 0 ? SerializeSafe(NewValues) : null,
            changedColumns: ChangedColumns.Count > 0 ? JsonSerializer.Serialize(ChangedColumns, JsonOptions) : null,
            ipAddress: IpAddress,
            correlationId: CorrelationId
        );
    }

    /// <summary>
    /// Dictionary'yi güvenli şekilde serialize eder.
    /// Smart Enum ve kompleks tipleri basitleştirir.
    /// </summary>
    private static string SerializeSafe(Dictionary<string, object?> values)
    {
        var simplifiedValues = new Dictionary<string, object?>();

        foreach (var kvp in values)
        {
            simplifiedValues[kvp.Key] = SimplifyValue(kvp.Value);
        }

        return JsonSerializer.Serialize(simplifiedValues, JsonOptions);
    }

    /// <summary>
    /// Değeri serialize edilebilir hale getirir.
    /// Smart Enum'ları ID ve Name olarak basitleştirir.
    /// </summary>
    private static object? SimplifyValue(object? value)
    {
        if (value == null)
            return null;

        var type = value.GetType();

        // Primitive types - olduğu gibi döndür
        if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal) ||
            type == typeof(DateTime) || type == typeof(DateTimeOffset) ||
            type == typeof(Guid) || type == typeof(TimeSpan))
        {
            return value;
        }

        // Nullable primitive
        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType != null && (underlyingType.IsPrimitive ||
            underlyingType == typeof(decimal) || underlyingType == typeof(DateTime) ||
            underlyingType == typeof(Guid)))
        {
            return value;
        }

        // Enumeration (Smart Enum) - sadece Id ve Name döndür
        if (IsSmartEnum(type))
        {
            var idProp = type.GetProperty("Id");
            var nameProp = type.GetProperty("Name") ?? type.GetProperty("DisplayName");

            if (idProp != null)
            {
                var id = idProp.GetValue(value);
                var name = nameProp?.GetValue(value)?.ToString();
                return name != null ? $"{id}:{name}" : id;
            }
        }

        // Regular Enum
        if (type.IsEnum)
        {
            return $"{(int)value}:{value}";
        }

        // Diğer kompleks tipler - ToString() kullan
        return value.ToString();
    }

    /// <summary>
    /// Tip Smart Enum (Enumeration base class) mı kontrol eder
    /// </summary>
    private static bool IsSmartEnum(Type type)
    {
        // Enumeration base class'ını kontrol et
        var baseType = type.BaseType;
        while (baseType != null)
        {
            if (baseType.IsGenericType &&
                baseType.GetGenericTypeDefinition().Name.StartsWith("Enumeration"))
            {
                return true;
            }
            if (baseType.Name == "Enumeration")
            {
                return true;
            }
            baseType = baseType.BaseType;
        }
        return false;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}