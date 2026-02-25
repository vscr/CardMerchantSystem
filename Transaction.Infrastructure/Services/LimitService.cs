using Transaction.Domain.Services;
using Transaction.Domain.ValueObjects;
using CardMerchantSystem.Shared.Kernel;
using StackExchange.Redis;

namespace Transaction.Infrastructure.Services;

/// <summary>
/// Redis Lua Script tabanlı Atomic Limit Kontrol Sistemi (LKS)
/// 
/// Neden Lua Script?
/// → Check + Reserve iki ayrı Redis komutu olunca TOCTOU race condition oluşur.
/// → Lua script Redis'te tek atomic operasyon olarak çalışır.
/// → İki POS aynı anda aynı kartla işlem yapsa bile limit aşılmaz.
/// 
/// Mülakat: "Concurrent işlemlerde limit nasıl korunuyor?"
/// → "Redis Lua script ile check + increment tek atomic operasyonda yapılıyor.
///    EVAL server-side çalışır, araya başka komut giremez."
/// </summary>
public class LimitService : ILimitService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    private const string KEY_PREFIX = "card_limit:";

    // ═══════════════════════════════════════════════════════════════
    // Lua Scripts — Redis'te atomic olarak çalışır
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Atomic Check + Reserve: Limit kontrolü + artırma tek operasyonda.
    /// 
    /// KEYS[1] = daily key, KEYS[2] = monthly key
    /// ARGV[1] = amount (kuruş), ARGV[2] = daily limit, ARGV[3] = monthly limit
    /// ARGV[4] = daily TTL (saniye), ARGV[5] = monthly TTL (saniye)
    /// 
    /// Return: 0 = OK, 1 = daily exceeded, 2 = monthly exceeded
    /// </summary>
    private static readonly LuaScript _checkAndReserveScript = LuaScript.Prepare(@"
        local dailyUsed = tonumber(redis.call('GET', @dailyKey) or '0')
        local monthlyUsed = tonumber(redis.call('GET', @monthlyKey) or '0')
        local amount = tonumber(@amount)
        local dailyLimit = tonumber(@dailyLimit)
        local monthlyLimit = tonumber(@monthlyLimit)

        -- Check
        if dailyUsed + amount > dailyLimit then
            return 1
        end
        if monthlyUsed + amount > monthlyLimit then
            return 2
        end

        -- Reserve (atomic increment)
        redis.call('INCRBY', @dailyKey, amount)
        redis.call('EXPIRE', @dailyKey, tonumber(@dailyTtl))
        redis.call('INCRBY', @monthlyKey, amount)
        redis.call('EXPIRE', @monthlyKey, tonumber(@monthlyTtl))

        return 0
    ");

    /// <summary>
    /// Atomic Refund: GET-SUBTRACT-SET tek operasyonda.
    /// Araya başka komut giremez → tutarsızlık imkansız.
    /// </summary>
    private static readonly LuaScript _refundScript = LuaScript.Prepare(@"
        local dailyUsed = tonumber(redis.call('GET', @dailyKey) or '0')
        local monthlyUsed = tonumber(redis.call('GET', @monthlyKey) or '0')
        local amount = tonumber(@amount)

        local newDaily = math.max(0, dailyUsed - amount)
        local newMonthly = math.max(0, monthlyUsed - amount)

        redis.call('SET', @dailyKey, tostring(newDaily))
        redis.call('EXPIRE', @dailyKey, tonumber(@dailyTtl))
        redis.call('SET', @monthlyKey, tostring(newMonthly))
        redis.call('EXPIRE', @monthlyKey, tonumber(@monthlyTtl))

        return 1
    ");

    // Default limitler (ICardLimitProvider'dan gelmezse)
    private const decimal DEFAULT_DAILY_LIMIT = 10000m;
    private const decimal DEFAULT_MONTHLY_LIMIT = 50000m;

    public LimitService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = _redis.GetDatabase();
    }

    public async Task<Result<CardLimit>> GetCardLimitAsync(string cardNumber, CancellationToken cancellationToken = default)
    {
        var dailyKey = GetDailyKey(cardNumber);
        var monthlyKey = GetMonthlyKey(cardNumber);

        var dailyUsed = await GetDecimalValue(dailyKey);
        var monthlyUsed = await GetDecimalValue(monthlyKey);

        var limit = CardLimit.Create(
            cardNumber,
            DEFAULT_DAILY_LIMIT,
            DEFAULT_MONTHLY_LIMIT,
            dailyUsed,
            monthlyUsed);

        return limit;
    }

    /// <summary>
    /// Atomic Check + Reserve — Lua script ile tek operasyonda.
    /// Eski: CheckLimitAsync + ReserveLimitAsync (iki ayrı çağrı, race condition)
    /// Yeni: Tek çağrı, atomic, güvenli.
    /// </summary>
    public async Task<Result> CheckAndReserveLimitAsync(
        string cardNumber, decimal amount, string transactionId, CancellationToken cancellationToken = default)
    {
        var dailyKey = GetDailyKey(cardNumber);
        var monthlyKey = GetMonthlyKey(cardNumber);
        var amountInKurus = (long)(amount * 100);
        var dailyLimitInKurus = (long)(DEFAULT_DAILY_LIMIT * 100);
        var monthlyLimitInKurus = (long)(DEFAULT_MONTHLY_LIMIT * 100);

        var result = (int)await _db.ScriptEvaluateAsync(_checkAndReserveScript, new
        {
            dailyKey = (RedisKey)dailyKey,
            monthlyKey = (RedisKey)monthlyKey,
            amount = amountInKurus,
            dailyLimit = dailyLimitInKurus,
            monthlyLimit = monthlyLimitInKurus,
            dailyTtl = (int)GetEndOfDay().TotalSeconds,
            monthlyTtl = (int)GetEndOfMonth().TotalSeconds
        });

        return result switch
        {
            0 => Result.Success(),
            1 => Result.Failure($"Günlük limit aşıldı. Limit: {DEFAULT_DAILY_LIMIT:N2} TRY", ErrorCodes.LimitExceeded),
            2 => Result.Failure($"Aylık limit aşıldı. Limit: {DEFAULT_MONTHLY_LIMIT:N2} TRY", ErrorCodes.LimitExceeded),
            _ => Result.Failure("Limit kontrolü hatası", ErrorCodes.SystemError)
        };
    }

    // ── Eski metodlar (geriye uyumluluk + yeni atomic versiyonlar) ──

    public async Task<Result> CheckLimitAsync(string cardNumber, decimal amount, CancellationToken cancellationToken = default)
    {
        // Sadece kontrol — reserve yapmaz
        var dailyUsed = await GetDecimalValue(GetDailyKey(cardNumber));
        var monthlyUsed = await GetDecimalValue(GetMonthlyKey(cardNumber));

        if (dailyUsed + (amount * 100) > DEFAULT_DAILY_LIMIT * 100)
            return Result.Failure($"Günlük limit aşıldı. Kalan: {(DEFAULT_DAILY_LIMIT - dailyUsed / 100):N2} TRY",
                ErrorCodes.LimitExceeded);

        if (monthlyUsed + (amount * 100) > DEFAULT_MONTHLY_LIMIT * 100)
            return Result.Failure($"Aylık limit aşıldı. Kalan: {(DEFAULT_MONTHLY_LIMIT - monthlyUsed / 100):N2} TRY",
                ErrorCodes.LimitExceeded);

        return Result.Success();
    }

    public async Task<Result> ReserveLimitAsync(string cardNumber, decimal amount, string transactionId, CancellationToken cancellationToken = default)
    {
        // Reserve key ile izle (timeout rollback için)
        var reserveKey = GetReserveKey(cardNumber, transactionId);
        await _db.StringSetAsync(reserveKey, ((long)(amount * 100)).ToString(), TimeSpan.FromMinutes(30));
        return Result.Success();
    }

    public async Task<Result> CommitLimitAsync(string cardNumber, decimal amount, string transactionId, CancellationToken cancellationToken = default)
    {
        // Reserve key'i sil (limit zaten Lua ile artırıldı)
        var reserveKey = GetReserveKey(cardNumber, transactionId);
        await _db.KeyDeleteAsync(reserveKey);
        return Result.Success();
    }

    public async Task<Result> ReleaseLimitAsync(string cardNumber, decimal amount, string transactionId, CancellationToken cancellationToken = default)
    {
        // İşlem reddedildi → Lua ile artırılan limiti geri al
        var dailyKey = GetDailyKey(cardNumber);
        var monthlyKey = GetMonthlyKey(cardNumber);
        var amountInKurus = (long)(amount * 100);

        await _db.ScriptEvaluateAsync(_refundScript, new
        {
            dailyKey = (RedisKey)dailyKey,
            monthlyKey = (RedisKey)monthlyKey,
            amount = amountInKurus,
            dailyTtl = (int)GetEndOfDay().TotalSeconds,
            monthlyTtl = (int)GetEndOfMonth().TotalSeconds
        });

        var reserveKey = GetReserveKey(cardNumber, transactionId);
        await _db.KeyDeleteAsync(reserveKey);

        return Result.Success();
    }

    public async Task<Result> RefundLimitAsync(string cardNumber, decimal amount, string transactionId, CancellationToken cancellationToken = default)
    {
        // İade işlemi → Lua script ile atomic azalt
        var dailyKey = GetDailyKey(cardNumber);
        var monthlyKey = GetMonthlyKey(cardNumber);
        var amountInKurus = (long)(amount * 100);

        await _db.ScriptEvaluateAsync(_refundScript, new
        {
            dailyKey = (RedisKey)dailyKey,
            monthlyKey = (RedisKey)monthlyKey,
            amount = amountInKurus,
            dailyTtl = (int)GetEndOfDay().TotalSeconds,
            monthlyTtl = (int)GetEndOfMonth().TotalSeconds
        });

        return Result.Success();
    }

    public async Task<Result> ResetDailyLimitsAsync(CancellationToken cancellationToken = default)
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: $"{KEY_PREFIX}daily:*").ToArray();
        foreach (var key in keys)
            await _db.KeyDeleteAsync(key);
        return Result.Success();
    }

    public async Task<Result> ResetMonthlyLimitsAsync(CancellationToken cancellationToken = default)
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: $"{KEY_PREFIX}monthly:*").ToArray();
        foreach (var key in keys)
            await _db.KeyDeleteAsync(key);
        return Result.Success();
    }

    // ── Helpers ──

    private static string GetDailyKey(string cardNumber) =>
        $"{KEY_PREFIX}daily:{cardNumber}:{DateTime.UtcNow:yyyyMMdd}";

    private static string GetMonthlyKey(string cardNumber) =>
        $"{KEY_PREFIX}monthly:{cardNumber}:{DateTime.UtcNow:yyyyMM}";

    private static string GetReserveKey(string cardNumber, string transactionId) =>
        $"{KEY_PREFIX}reserve:{cardNumber}:{transactionId}";

    private async Task<decimal> GetDecimalValue(string key)
    {
        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty) return 0m;
        if (long.TryParse(value, out var longValue)) return longValue / 100m;
        return 0m;
    }

    private static TimeSpan GetEndOfDay()
    {
        var now = DateTime.UtcNow;
        return now.Date.AddDays(1) - now;
    }

    private static TimeSpan GetEndOfMonth()
    {
        var now = DateTime.UtcNow;
        return new DateTime(now.Year, now.Month, 1).AddMonths(1) - now;
    }
}