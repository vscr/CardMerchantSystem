using Fee.Domain.Entities;
using CardMerchantSystem.Shared.Kernel;

namespace Fee.Domain.Services;

/// <summary>
/// Ücret Hesaplama Servisi
/// </summary>
public interface IFeeCalculationService
{
    /// <summary>
    /// İşlem komisyonu hesaplar
    /// </summary>
    Task<Result<TransactionFeeResult>> CalculateTransactionFeeAsync(
        string merchantId,
        decimal transactionAmount,
        string? mcc = null,
        int installmentCount = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Komisyon dağılımı oluşturur
    /// </summary>
    Task<Result<CommissionBreakdown>> CreateCommissionBreakdownAsync(
        Guid transactionId,
        string merchantId,
        decimal transactionAmount,
        string? mcc = null,
        int installmentCount = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Aidat tahakkuku oluşturur
    /// </summary>
    Task<Result<FeeAccrual>> CreateMembershipAccrualAsync(
        Guid membershipFeeId,
        string? merchantId = null,
        string? cardNumber = null,
        string? terminalId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gecikme faizi hesaplar
    /// </summary>
    Task<Result<decimal>> CalculateLateFeeAsync(
        Guid accrualId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// İşlem komisyon hesaplama sonucu
/// </summary>
public class TransactionFeeResult
{
    public decimal TransactionAmount { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal MerchantNetAmount { get; set; }
    public decimal CommissionRate { get; set; }
    public Guid TariffId { get; set; }
    public string TariffCode { get; set; } = null!;

    // Kırılım
    public decimal BankShare { get; set; }
    public decimal InterchangeFee { get; set; }
    public decimal BKMFee { get; set; }

    // Detaylar
    public string? MCC { get; set; }
    public int InstallmentCount { get; set; }
    public string CalculationType { get; set; } = null!;
}