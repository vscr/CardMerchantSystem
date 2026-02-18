namespace CardMerchantSystem.Shared.Idempotency;

/// <summary>
/// Idempotency kaydı — işlenmiş isteklerin kalıcı kaydı.
/// 
/// Bankacılıkta neden gerekli?
/// ───────────────────────────
/// POS terminalleri ağ kesintisi yaşadığında aynı işlemi tekrar gönderir.
/// Idempotency key ile ilk işlemin sonucunu cache'den döndürüp
/// mükerrer (duplicate) işlem oluşmasını engelliyoruz.
/// 
/// ISO 8583'te bu konsept "Original Data Element" (DE-90) ile sağlanır.
/// Modern REST API'lerde ise HTTP header (Idempotency-Key) kullanılır.
/// 
/// Yaşam döngüsü:
/// ──────────────
/// 1. İstek gelir → Redis'te key var mı? (hızlı kontrol)
/// 2. Yoksa → Redis'e "PROCESSING" yaz (distributed lock)
/// 3. İşlem tamamlanır → Redis + DB'ye sonucu yaz
/// 4. Aynı key tekrar gelirse → Cache'den sonuç dön (işlem yapma)
/// 5. TTL dolunca → Redis'ten silinir, DB'de kalır (audit)
/// 
/// Neden Redis + DB hybrid?
/// ────────────────────────
/// - Redis: Hız (< 1ms lookup), distributed lock, TTL
/// - DB: Kalıcı kayıt (audit trail), Redis crash recovery
/// </summary>
public class IdempotencyRecord
{
    /// <summary>
    /// Client tarafından gönderilen unique key.
    /// Format: {TerminalCode}:{RRN} veya UUID
    /// 
    /// Bankacılık örnekleri:
    /// - POS: "T1017506:260214123456" (terminal + RRN)
    /// - ATM: "ATM001:260214:0001" (ATM + tarih + sıra no)
    /// - Online: "ORD-2026-00123" (sipariş numarası)
    /// </summary>
    public string IdempotencyKey { get; set; } = null!;

    /// <summary>
    /// Hangi API endpoint'i çağrıldı?
    /// Aynı key farklı endpoint'lerde kullanılabilir.
    /// </summary>
    public string RequestPath { get; set; } = null!;

    /// <summary>
    /// İsteğin SHA256 hash'i.
    /// 
    /// Neden gerekli?
    /// Aynı idempotency key ile FARKLI body gönderilirse
    /// bu bir hata veya saldırı girişimi olabilir.
    /// Hash uyuşmazlığında 409 Conflict dönüyoruz.
    /// 
    /// Örnek: Key="T1017506:001" ile 1000 TL gönderildi,
    /// sonra aynı key ile 5000 TL gönderilirse → 409
    /// </summary>
    public string RequestHash { get; set; } = null!;

    /// <summary>
    /// HTTP status code (200, 400, 500 vb.)
    /// </summary>
    public int ResponseStatusCode { get; set; }

    /// <summary>
    /// Serialize edilmiş response body (JSON).
    /// İlk işlemin tam sonucu burada saklanır.
    /// Tekrar istek geldiğinde bu döndürülür.
    /// </summary>
    public string? ResponseBody { get; set; }

    /// <summary>
    /// İşlem durumu.
    /// 
    /// State machine:
    /// Processing → Completed | Failed | Expired
    /// 
    /// "Processing" durumu distributed lock görevi görür:
    /// - İstek A geldi → status = Processing (lock alındı)
    /// - İstek B aynı key ile geldi → Processing görürse bekler
    /// - İstek A tamamlandı → status = Completed
    /// - İstek B tekrar kontrol → Completed görür, cache'den döner
    /// </summary>
    public string Status { get; set; } = "Processing";

    /// <summary>
    /// İşlem oluşturulduysa transaction ID'si.
    /// Audit trail ve troubleshooting için.
    /// </summary>
    public Guid? TransactionId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Ne zamana kadar geçerli?
    /// Default: 24 saat (bankacılık standardı).
    /// 
    /// Neden 24 saat?
    /// - POS gün sonu mutabakatı genelde gece yapılır
    /// - Aynı gün içinde retry olabilir
    /// - 24 saat sonra aynı key ile yeni işlem yapılabilir
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Kaç kez tekrar istek geldi?
    /// Yüksek retry count anomali göstergesi olabilir.
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// Son retry zamanı
    /// </summary>
    public DateTime? LastRetryAt { get; set; }
}