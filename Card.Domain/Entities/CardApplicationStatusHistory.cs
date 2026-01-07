using Card.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;

namespace Card.Domain.Entities;

/// <summary>
/// Başvuru durum geçmişi (Child Entity)
/// Her durum değişikliğinde bir kayıt oluşturulur.
/// </summary>
public class CardApplicationStatusHistory : Entity
{
    public Guid CardApplicationId { get; private set; }
    public CardApplicationStatus Status { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string ChangedBy { get; private set; } = null!;
    public DateTime ChangedAt { get; private set; }

    // EF Core için
    private CardApplicationStatusHistory() { }

    public CardApplicationStatusHistory(
        Guid cardApplicationId,
        CardApplicationStatus status,
        string description,
        string changedBy)
    {
        CardApplicationId = cardApplicationId;
        Status = status;
        Description = description;
        ChangedBy = changedBy;
        ChangedAt = DateTime.UtcNow;
    }
}