using CardMerchantSystem.Shared.Kernel;

namespace Dispute.Domain.Entities;

/// <summary>
/// İtiraz notu
/// </summary>
public class DisputeNote : Entity
{
    public Guid DisputeId { get; private set; }
    public string Note { get; private set; } = null!;
    public string CreatedByUser { get; private set; } = null!;
    public bool IsInternal { get; private set; }

    // EF Core için
    private DisputeNote() { }

    public static DisputeNote Create(
        Guid disputeId,
        string note,
        string createdByUser,
        bool isInternal = true)
    {
        return new DisputeNote
        {
            DisputeId = disputeId,
            Note = note,
            CreatedByUser = createdByUser,
            IsInternal = isInternal
        };
    }
}