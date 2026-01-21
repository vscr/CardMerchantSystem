namespace CardMerchantSystem.Shared.Kernel.Exceptions;

/// <summary>
/// Kayıt bulunamadığında fırlatılır
/// </summary>
public class NotFoundException : DomainException
{
    public string EntityName { get; }
    public object? EntityId { get; }

    public NotFoundException(string entityName, object? entityId = null)
        : base($"{entityName} bulunamadı{(entityId != null ? $": {entityId}" : "")}", "NOT_FOUND")
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}