using CardMerchantSystem.Shared.Idempotency;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;
using Transaction.Infrastructure.Persistence;

namespace Transaction.Infrastructure.Idempotency;

/// <summary>
/// Redis + DB hybrid idempotency store.
/// 
/// Neden hybrid?
/// ─────────────
/// Sadece Redis: Hızlı ama volatile (restart'ta kaybolur)
/// Sadece DB: Kalıcı ama yavaş (her istek DB sorgusu)
/// Hybrid: Redis'ten oku (< 1ms), DB'ye yaz (audit + recovery)
/// 
/// Concurrent request handling:
/// ────────────────────────────
/// İstek A ve B aynı anda aynı key ile gelirse:
/// 
/// T1: A → Redis SET NX "key" "PROCESSING" → SUCCESS (lock aldı)
/// T2: B → Redis SET NX "key" "PROCESSING" → FAIL (key var!)
/// T3: B → Redis GET "key" → "PROCESSING" (A hâlâ çalışıyor)
/// T4: B → 409 Conflict veya bekle+retry
/// T5: A → tamamlandı → Redis SET "key" {response} + DB INSERT
/// T6: B → tekrar gelirse → Redis GET → response (cache hit)
/// 
/// SET NX = "Set if Not eXists" — Redis'in atomic lock mekanizması
/// Bu sayede race condition asla olmaz.
/// </summary>
public class IdempotencyStore : IIdempotencyStore
{
    private readonly IDatabase _redis;
    private readonly TransactionDbContext _dbContext;
    private readonly IdempotencyOptions _options;
    private readonly ILogger<IdempotencyStore> _logger;

    private const string REDIS_PREFIX = "idempotency:";

    public IdempotencyStore(
        IConnectionMultiplexer redis,
        TransactionDbContext dbContext,
        IOptions<IdempotencyOptions> options,
        ILogger<IdempotencyStore> logger)
    {
        _redis = redis.GetDatabase();
        _dbContext = dbContext;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// 1. Redis'ten oku (hot path — < 1ms)
    /// 2. Yoksa DB'den oku (cold path — ~5ms)
    /// 3. DB'de varsa Redis'e geri yaz (cache warm)
    /// </summary>
    public async Task<IdempotencyRecord?> GetAsync(string key, CancellationToken ct = default)
    {
        // Layer 1: Redis
        var redisKey = $"{REDIS_PREFIX}{key}";
        var cached = await _redis.StringGetAsync(redisKey);

        if (!cached.IsNullOrEmpty)
        {
            var record = JsonSerializer.Deserialize<IdempotencyRecord>(cached!);

            // Processing timeout kontrolü
            if (record?.Status == "Processing" &&
                record.CreatedAt.Add(_options.ProcessingTimeout) < DateTime.UtcNow)
            {
                _logger.LogWarning(
                    "Idempotency key processing timeout: {Key}, Created: {Created}",
                    key, record.CreatedAt);

                // Lock'u temizle — yeni istek işleyebilsin
                await _redis.KeyDeleteAsync(redisKey);
                return null;
            }

            return record;
        }

        // Layer 2: DB (Redis miss — restart sonrası veya evict)
        var dbRecord = await _dbContext.IdempotencyRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdempotencyKey == key, ct);

        if (dbRecord != null && dbRecord.ExpiresAt > DateTime.UtcNow)
        {
            // Redis'e geri yaz (cache warm)
            var ttl = dbRecord.ExpiresAt - DateTime.UtcNow;
            var json = JsonSerializer.Serialize(dbRecord);
            await _redis.StringSetAsync(redisKey, json, ttl);
        }

        return dbRecord;
    }

    /// <summary>
    /// Redis SET NX ile atomic lock alma.
    /// 
    /// SET NX (Set if Not eXists):
    /// - Key yoksa → SET yapar, true döner (lock başarılı)
    /// - Key varsa → hiçbir şey yapmaz, false döner (başkası işliyor)
    /// 
    /// Bu işlem Redis'te ATOMIC — thread-safe, race condition yok.
    /// Distributed sistemlerde (çoklu API instance) bile güvenli.
    /// </summary>
    public async Task<bool> TryCreateAsync(IdempotencyRecord record, CancellationToken ct = default)
    {
        var redisKey = $"{REDIS_PREFIX}{record.IdempotencyKey}";
        var json = JsonSerializer.Serialize(record);

        // SET NX — atomic lock
        var acquired = await _redis.StringSetAsync(
            redisKey,
            json,
            _options.ProcessingTimeout,   // Lock süresi (30s)
            When.NotExists);              // NX — sadece yoksa

        if (!acquired)
        {
            _logger.LogInformation(
                "Idempotency key zaten mevcut (concurrent request): {Key}", record.IdempotencyKey);
            return false;
        }

        // DB'ye de yaz (kalıcı kayıt)
        try
        {
            _dbContext.IdempotencyRecords.Add(record);
            await _dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            // DB'de zaten varsa (unique constraint) → sorun değil, Redis lock yeterli
            _logger.LogDebug("Idempotency DB kaydı zaten mevcut: {Key}", record.IdempotencyKey);
        }

        return true;
    }

    /// <summary>
    /// İşlem başarılı — sonucu hem Redis hem DB'ye yaz.
    /// Redis TTL = 24 saat (sıcak cache)
    /// DB'de kalıcı (audit trail)
    /// </summary>
    public async Task CompleteAsync(string key, int statusCode, string responseBody,
        Guid? transactionId = null, CancellationToken ct = default)
    {
        var redisKey = $"{REDIS_PREFIX}{key}";

        // DB güncelle
        var dbRecord = await _dbContext.IdempotencyRecords
            .FirstOrDefaultAsync(x => x.IdempotencyKey == key, ct);

        if (dbRecord != null)
        {
            dbRecord.Status = "Completed";
            dbRecord.ResponseStatusCode = statusCode;
            dbRecord.ResponseBody = responseBody;
            dbRecord.TransactionId = transactionId;
            dbRecord.CompletedAt = DateTime.UtcNow;
            dbRecord.ExpiresAt = DateTime.UtcNow.Add(_options.DefaultTtl);

            await _dbContext.SaveChangesAsync(ct);

            // Redis güncelle (tam TTL ile)
            var json = JsonSerializer.Serialize(dbRecord);
            await _redis.StringSetAsync(redisKey, json, _options.DefaultTtl);
        }

        _logger.LogInformation(
            "Idempotency completed: Key={Key}, Status={StatusCode}, TxId={TransactionId}",
            key, statusCode, transactionId);
    }

    /// <summary>
    /// İşlem hata aldı — hata da cache'lenmeli!
    /// 
    /// Neden hata da idempotent?
    /// Aynı hatalı istek tekrar gelirse aynı hata dönmeli.
    /// Yoksa her retry'da yeni hata kaydı oluşur.
    /// 
    /// Önemli: Transient (geçici) hatalar cache'lenmez!
    /// 500 Internal Error → cache'leme (retry farklı sonuç verebilir)
    /// 400 Bad Request → cache'le (aynı hatalı veri aynı hatayı verir)
    /// </summary>
    public async Task FailAsync(string key, int statusCode, string responseBody,
        CancellationToken ct = default)
    {
        var redisKey = $"{REDIS_PREFIX}{key}";

        var dbRecord = await _dbContext.IdempotencyRecords
            .FirstOrDefaultAsync(x => x.IdempotencyKey == key, ct);

        if (dbRecord != null)
        {
            // 5xx hatalar → lock'u serbest bırak (retry yapılabilsin)
            if (statusCode >= 500)
            {
                dbRecord.Status = "Failed";
                await _redis.KeyDeleteAsync(redisKey);
                _logger.LogWarning(
                    "Idempotency failed (transient, lock released): Key={Key}", key);
            }
            else
            {
                // 4xx hatalar → cache'le (aynı hatalı veri aynı hatayı verir)
                dbRecord.Status = "Completed";
                dbRecord.ResponseStatusCode = statusCode;
                dbRecord.ResponseBody = responseBody;
                dbRecord.CompletedAt = DateTime.UtcNow;
                dbRecord.ExpiresAt = DateTime.UtcNow.Add(_options.DefaultTtl);

                var json = JsonSerializer.Serialize(dbRecord);
                await _redis.StringSetAsync(redisKey, json, _options.DefaultTtl);
            }

            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task IncrementRetryAsync(string key, CancellationToken ct = default)
    {
        var dbRecord = await _dbContext.IdempotencyRecords
            .FirstOrDefaultAsync(x => x.IdempotencyKey == key, ct);

        if (dbRecord != null)
        {
            dbRecord.RetryCount++;
            dbRecord.LastRetryAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(ct);

            if (dbRecord.RetryCount >= _options.MaxRetryWarningThreshold)
            {
                _logger.LogWarning(
                    "Idempotency yüksek retry: Key={Key}, Count={Count} — olası POS/ağ sorunu",
                    key, dbRecord.RetryCount);
            }
        }
    }

    /// <summary>
    /// Hangfire job: Süresi dolmuş kayıtları temizle.
    /// DB'den sil (audit ihtiyacına göre archive tablosuna taşınabilir).
    /// </summary>
    public async Task CleanupExpiredAsync(CancellationToken ct = default)
    {
        var expiredCount = await _dbContext.IdempotencyRecords
            .Where(x => x.ExpiresAt < DateTime.UtcNow)
            .ExecuteDeleteAsync(ct);

        if (expiredCount > 0)
            _logger.LogInformation("Idempotency cleanup: {Count} expired records deleted", expiredCount);
    }
}