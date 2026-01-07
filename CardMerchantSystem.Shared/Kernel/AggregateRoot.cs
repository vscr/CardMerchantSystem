namespace CardMerchantSystem.Shared.Kernel;

/// <summary>
/// Aggregate Root base class.
/// </summary>
public abstract class AggregateRoot : Entity
{
    protected AggregateRoot() : base()
    {
    }

    protected AggregateRoot(Guid id) : base(id)
    {
    }
}