using Card.Domain.Entities;
using Card.Domain.Enums;
using Card.Domain.ValueObjects;

namespace Card.Domain.Repositories;

/// <summary>
/// Kart başvurusu repository interface.
/// Domain layer'da tanımlanır, Infrastructure'da implement edilir.
/// </summary>
public interface ICardApplicationRepository
{
    Task<(IReadOnlyList<CardApplication> Items, int TotalCount)> GetPagedAsync(
    int pageNumber,
    int pageSize,
    int? statusId = null,
    int? cardTypeId = null,
    string? customerTckn = null,
    string? customerName = null,
    DateTime? startDate = null,
    DateTime? endDate = null,
    string? sortBy = null,
    bool sortDescending = false,
    CancellationToken cancellationToken = default);

    Task<CardApplication?> GetByMaskedCardNoAsync(string maskedCardNo, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CardApplication>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CardApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CardApplication?> GetByIdWithHistoryAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CardApplication>> GetByCustomerTcknAsync(TCKN tckn, CancellationToken cancellationToken = default);

    Task<bool> HasActiveApplicationAsync(TCKN tckn, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CardApplication>> GetByStatusAsync(CardApplicationStatus status, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CardApplication>> GetByPrintBatchIdAsync(string batchId, CancellationToken cancellationToken = default);

    Task AddAsync(CardApplication application, CancellationToken cancellationToken = default);

    Task UpdateAsync(CardApplication application, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}