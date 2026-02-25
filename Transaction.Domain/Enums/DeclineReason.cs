using CardMerchantSystem.Shared.Kernel;

namespace Transaction.Domain.Enums;

/// <summary>
/// Red nedenleri
/// </summary>
public class DeclineReason : Enumeration
{
    public static readonly DeclineReason None = new(0, nameof(None), "Yok");
    public static readonly DeclineReason InsufficientLimit = new(1, nameof(InsufficientLimit), "Yetersiz Limit");
    public static readonly DeclineReason CardBlocked = new(2, nameof(CardBlocked), "Kart Blokeli");
    public static readonly DeclineReason CardExpired = new(3, nameof(CardExpired), "Kart Süresi Dolmuş");
    public static readonly DeclineReason InvalidCard = new(4, nameof(InvalidCard), "Geçersiz Kart");
    public static readonly DeclineReason InvalidMerchant = new(5, nameof(InvalidMerchant), "Geçersiz Üye İşyeri");
    public static readonly DeclineReason InvalidTerminal = new(6, nameof(InvalidTerminal), "Geçersiz Terminal");
    public static readonly DeclineReason FraudSuspected = new(7, nameof(FraudSuspected), "Fraud Şüphesi");
    public static readonly DeclineReason DailyLimitExceeded = new(8, nameof(DailyLimitExceeded), "Günlük Limit Aşıldı");
    public static readonly DeclineReason MonthlyLimitExceeded = new(9, nameof(MonthlyLimitExceeded), "Aylık Limit Aşıldı");
    public static readonly DeclineReason TransactionNotPermitted = new(10, nameof(TransactionNotPermitted), "İşlem İzni Yok");
    public static readonly DeclineReason SystemError = new(11, nameof(SystemError), "Sistem Hatası");
    public static readonly DeclineReason RestrictedCard = new(4, nameof(RestrictedCard), "Kısıtlı Kart");


    private DeclineReason(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }
}