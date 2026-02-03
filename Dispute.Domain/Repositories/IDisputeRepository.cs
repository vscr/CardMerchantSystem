using Dispute.Domain.Entities;
using Dispute.Domain.Enums;

namespace Dispute.Domain.Repositories;

/// <summary>
/// İtiraz repository interface
/// </summary>
public interface IDisputeRepository
{
    Task<DisputeAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<DisputeAggregate?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<DisputeAggregate?> GetByDisputeNumberAsync(string disputeNumber, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DisputeAggregate>> GetByTransactionIdAsync(Guid transactionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DisputeAggregate>> GetByMerchantIdAsync(Guid merchantId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DisputeAggregate>> GetByCustomerTcknAsync(string customerTckn, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DisputeAggregate>> GetByStatusAsync(DisputeStatus status, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DisputeAggregate>> GetOverdueDisputesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DisputeAggregate>> GetAssignedToUserAsync(string username, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DisputeAggregate>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sayfalı itiraz listesi getirir
    /// </summary>
    Task<(IReadOnlyList<DisputeAggregate> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        DisputeStatus? status = null,
        Guid? merchantId = null,
        string? customerTckn = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        bool? isOverdue = null,
        string? assignedTo = null,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default);

    Task AddAsync(DisputeAggregate dispute, CancellationToken cancellationToken = default);

    Task UpdateAsync(DisputeAggregate dispute, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}