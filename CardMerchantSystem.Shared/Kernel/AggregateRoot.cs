namespace CardMerchantSystem.Shared.Kernel;

/// <summary>
/// Aggregate Root base class.
/// Her Aggregate'in tek bir root'u olur ve dış dünya sadece root üzerinden erişir.
/// </summary>
public abstract class AggregateRoot : Entity
{
    /// <summary>
    /// Optimistic concurrency için version numarası.
    /// Her update'de otomatik artar.
    /// </summary>
    public int Version { get; private set; }

    protected AggregateRoot() : base()
    {
        Version = 1;
    }

    protected AggregateRoot(Guid id) : base(id)
    {
        Version = 1;
    }

    public void IncrementVersion()
    {
        Version++;
    }
}