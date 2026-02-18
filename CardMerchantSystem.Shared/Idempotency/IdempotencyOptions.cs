namespace CardMerchantSystem.Shared.Idempotency;

/// <summary>
/// Idempotency ayarları.
/// appsettings.json'dan okunur.
/// </summary>
public class IdempotencyOptions
{
    public const string SectionName = "Idempotency";

    /// <summary>
    /// Idempotency kaydının geçerlilik süresi.
    /// Bankacılık standardı: 24 saat (gün sonu mutabakatı).
    /// </summary>
    public TimeSpan DefaultTtl { get; set; } = TimeSpan.FromHours(24);

    /// <summary>
    /// Processing durumundaki kaydın timeout süresi.
    /// Bu süre dolduğunda lock otomatik kalkar.
    /// 
    /// Neden gerekli?
    /// İstek A processing'e düştü ama crash oldu → lock sonsuza kadar kalır.
    /// Timeout ile lock otomatik temizlenir.
    /// </summary>
    public TimeSpan ProcessingTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// HTTP header adı. Stripe ve modern API'lerde standart.
    /// </summary>
    public string HeaderName { get; set; } = "Idempotency-Key";

    /// <summary>
    /// Maksimum retry sayısı. Aşılırsa uyarı logu.
    /// </summary>
    public int MaxRetryWarningThreshold { get; set; } = 5;
}