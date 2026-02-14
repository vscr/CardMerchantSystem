namespace CardMerchantSystem.Shared.Services;

/// <summary>
/// Kart limit bilgisi sağlayıcı.
/// Card modülü implement eder, Transaction modülü consume eder.
/// Modüller arası loosely-coupled iletişim.
/// </summary>
public interface ICardLimitProvider
{
    Task<CardLimitInfo?> GetCardLimitAsync(string maskedCardNo, CancellationToken ct = default);
}

public class CardLimitInfo
{
    public string MaskedCardNo { get; set; } = null!;
    public decimal DailyLimit { get; set; }
    public decimal MonthlyLimit { get; set; }
    public string Currency { get; set; } = "TRY";
}