using MediatR;

namespace CardMerchantSystem.Shared.Kernel;

/// <summary>
/// Domain Event marker interface.
/// Domain içinde olan önemli olayları temsil eder.
/// </summary>
public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
    Guid EventId { get; }
}

/// <summary>
/// Domain Event base implementation
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; }
    public Guid EventId { get; }

    protected DomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
        EventId = Guid.NewGuid();
    }
}