namespace CardMerchantSystem.Shared.Idempotency;

/// <summary>
/// Idempotency kayıt deposu interface'i.
/// 
/// İki katmanlı depolama stratejisi:
/// ─────────────────────────────────
/// Layer 1 - Redis (hot): Hızlı lookup + distributed lock + TTL
/// Layer 2 - DB (cold): Kalıcı kayıt + audit trail + recovery
/// 
/// Read path:  Redis → (miss) → DB → (miss) → null (yeni istek)
/// Write path: Redis + DB (dual write)
/// </summary>
public interface IIdempotencyStore
{
    /// <summary>
    /// Key ile kayıt getir (önce Redis, sonra DB)
    /// </summary>
    Task<IdempotencyRecord?> GetAsync(string key, CancellationToken ct = default);

    /// <summary>
    /// Yeni processing kaydı oluştur (distributed lock).
    /// Returns false if key already exists (başka istek işliyor).
    /// 
    /// Redis'te SET NX (Set if Not eXists) kullanır — atomic operation.
    /// Bu sayede race condition olmaz.
    /// </summary>
    Task<bool> TryCreateAsync(IdempotencyRecord record, CancellationToken ct = default);

    /// <summary>
    /// İşlem tamamlandı — sonucu kaydet (Redis + DB)
    /// </summary>
    Task CompleteAsync(string key, int statusCode, string responseBody,
        Guid? transactionId = null, CancellationToken ct = default);

    /// <summary>
    /// İşlem hata aldı — hata durumunu kaydet.
    /// Hatalı işlemler de idempotent olmalı:
    /// Aynı key ile tekrar gelirse aynı hatayı dönmeli.
    /// </summary>
    Task FailAsync(string key, int statusCode, string responseBody,
        CancellationToken ct = default);

    /// <summary>
    /// Retry sayacını artır
    /// </summary>
    Task IncrementRetryAsync(string key, CancellationToken ct = default);

    /// <summary>
    /// Süresi dolmuş kayıtları temizle (Hangfire job)
    /// </summary>
    Task CleanupExpiredAsync(CancellationToken ct = default);
}