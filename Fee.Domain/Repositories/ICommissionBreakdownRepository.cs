using Fee.Domain.Entities;
using Fee.Domain.Enums;
using Fee.Domain.Services;

namespace Fee.Domain.Repositories;

/// <summary>
/// Komisyon Dağılımı Repository Interface
/// </summary>
public interface ICommissionBreakdownRepository
{
    Task<CommissionBreakdown?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CommissionBreakdown?> GetByTransactionIdAsync(Guid transactionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CommissionBreakdown>> GetByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CommissionBreakdown>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CommissionBreakdown>> GetByMerchantAndDateRangeAsync(string merchantId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<MerchantCommissionSummary> GetMerchantSummaryAsync(string merchantId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task AddAsync(CommissionBreakdown breakdown, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Üye İşyeri Komisyon Özeti
/// </summary>
public class MerchantCommissionSummary
{
    public string MerchantId { get; set; } = null!;
    public int TransactionCount { get; set; }
    public decimal TotalTransactionAmount { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal TotalBankShare { get; set; }
    public decimal TotalInterchangeFee { get; set; }
    public decimal TotalBKMFee { get; set; }
    public decimal TotalMerchantNet { get; set; }
    public decimal AverageCommissionRate { get; set; }
}