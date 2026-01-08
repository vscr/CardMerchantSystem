using Transaction.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;

namespace Transaction.Domain.Services;

/// <summary>
/// Fraud kontrol servisi interface
/// </summary>
public interface IFraudService
{
    /// <summary>
    /// Fraud kontrolü yapar
    /// </summary>
    Task<Result<FraudCheckResponse>> CheckFraudAsync(FraudCheckRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Fraud kontrol isteği
/// </summary>
public class FraudCheckRequest
{
    public string CardNumberMasked { get; set; } = null!;
    public decimal Amount { get; set; }
    public string MerchantCode { get; set; } = null!;
    public string TerminalCode { get; set; } = null!;
    public string? IpAddress { get; set; }
    public string? DeviceId { get; set; }
    public DateTime TransactionTime { get; set; }
}

/// <summary>
/// Fraud kontrol yanıtı
/// </summary>
public class FraudCheckResponse
{
    public FraudCheckResult Result { get; set; } = null!;
    public int Score { get; set; }
    public List<string> Reasons { get; set; } = new();
}