using CardMerchantSystem.Shared.Kernel;
using CardMerchantSystem.Shared.Services;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Transaction.Domain.Entities;
using Transaction.Domain.Repositories;
using Transaction.Domain.Services;
using Transaction.Domain.ValueObjects;

namespace Transaction.Infrastructure.Services;

/// <summary>
/// Redis tabanlı Limit Kontrol Sistemi (LKS)
/// Limit tanımları DB'de, kullanımlar Redis'te.
/// 
/// Öncelik sırası:
/// 1. Kart bazlı özel limit (CARD)
/// 2. BIN bazlı limit (BIN)
/// 3. Global varsayılan limit (DEFAULT)
/// </summary>
public class LimitService : ILimitService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private readonly ICardLimitDefinitionRepository _limitRepo;
    private readonly ICardLimitProvider _cardLimitProvider;
    private readonly ILogger<LimitService> _logger;

    // Fallback limitler (DB erişilemezse)
    private const decimal FALLBACK_DAILY_LIMIT = 10000m;
    private const decimal FALLBACK_MONTHLY_LIMIT = 50000m;
    private const string KEY_PREFIX = "card_limit:";
    private const string LIMIT_DEF_CACHE_PREFIX = "limit_def:";
    private static readonly TimeSpan LimitDefCacheTtl = TimeSpan.FromMinutes(10);

    public LimitService(
        IConnectionMultiplexer redis,
        ICardLimitDefinitionRepository limitRepo,
        ILogger<LimitService> logger,
        ICardLimitProvider cardLimitProvider)
    {
        _redis = redis;
        _db = _redis.GetDatabase();
        _limitRepo = limitRepo;
        _logger = logger;
        _cardLimitProvider = cardLimitProvider;
    }

    public async Task<Result<CardLimit>> GetCardLimitAsync(string cardNumber, CancellationToken cancellationToken = default)
    {
        var (dailyLimit, monthlyLimit) = await ResolveCardLimitsAsync(cardNumber, cancellationToken);

        var dailyKey = GetDailyKey(cardNumber);
        var monthlyKey = GetMonthlyKey(cardNumber);

        var dailyUsed = await GetDecimalValue(dailyKey);
        var monthlyUsed = await GetDecimalValue(monthlyKey);

        var limit = CardLimit.Create(
            cardNumber,
            dailyLimit,
            monthlyLimit,
            dailyUsed,
            monthlyUsed);

        return limit;
    }

    public async Task<Result> CheckLimitAsync(string cardNumber, decimal amount, CancellationToken cancellationToken = default)
    {
        var limitResult = await GetCardLimitAsync(cardNumber, cancellationToken);
        if (limitResult.IsFailure)
            return Result.Failure(limitResult.Error!, limitResult.ErrorCode);

        var limit = limitResult.Value!;

        // Tek işlem limiti kontrolü
        var (_, _, singleLimit) = await ResolveCardLimitsWithSingleAsync(cardNumber, cancellationToken);
        if (singleLimit.HasValue && amount > singleLimit.Value)
            return Result.Failure($"Tek işlem limiti aşıldı. Maksimum: {singleLimit.Value:N2} TRY",
                ErrorCodes.LimitExceeded);

        if (amount > limit.RemainingDailyLimit)
            return Result.Failure($"Günlük limit aşıldı. Kalan limit: {limit.RemainingDailyLimit:N2} TRY",
                ErrorCodes.LimitExceeded);

        if (amount > limit.RemainingMonthlyLimit)
            return Result.Failure($"Aylık limit aşıldı. Kalan limit: {limit.RemainingMonthlyLimit:N2} TRY",
                ErrorCodes.LimitExceeded);

        return Result.Success();
    }

    public async Task<Result> ReserveLimitAsync(string cardNumber, decimal amount, string transactionId, CancellationToken cancellationToken = default)
    {
        var reserveKey = GetReserveKey(cardNumber, transactionId);
        await _db.StringSetAsync(reserveKey, amount.ToString(), TimeSpan.FromMinutes(30));
        return Result.Success();
    }

    public async Task<Result> CommitLimitAsync(string cardNumber, decimal amount, string transactionId, CancellationToken cancellationToken = default)
    {
        var dailyKey = GetDailyKey(cardNumber);
        var monthlyKey = GetMonthlyKey(cardNumber);
        var reserveKey = GetReserveKey(cardNumber, transactionId);

        await _db.StringIncrementAsync(dailyKey, (long)(amount * 100));
        await _db.KeyExpireAsync(dailyKey, GetEndOfDay());

        await _db.StringIncrementAsync(monthlyKey, (long)(amount * 100));
        await _db.KeyExpireAsync(monthlyKey, GetEndOfMonth());

        await _db.KeyDeleteAsync(reserveKey);

        return Result.Success();
    }

    public async Task<Result> ReleaseLimitAsync(string cardNumber, decimal amount, string transactionId, CancellationToken cancellationToken = default)
    {
        var reserveKey = GetReserveKey(cardNumber, transactionId);
        await _db.KeyDeleteAsync(reserveKey);
        return Result.Success();
    }

    public async Task<Result> RefundLimitAsync(string cardNumber, decimal amount, string transactionId, CancellationToken cancellationToken = default)
    {
        var dailyKey = GetDailyKey(cardNumber);
        var monthlyKey = GetMonthlyKey(cardNumber);

        var dailyUsed = await GetDecimalValue(dailyKey);
        var newDailyUsed = Math.Max(0, dailyUsed - amount);
        await _db.StringSetAsync(dailyKey, ((long)(newDailyUsed * 100)).ToString(), GetEndOfDay());

        var monthlyUsed = await GetDecimalValue(monthlyKey);
        var newMonthlyUsed = Math.Max(0, monthlyUsed - amount);
        await _db.StringSetAsync(monthlyKey, ((long)(newMonthlyUsed * 100)).ToString(), GetEndOfMonth());

        return Result.Success();
    }

    public async Task<Result> ResetDailyLimitsAsync(CancellationToken cancellationToken = default)
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: $"{KEY_PREFIX}daily:*").ToArray();
        foreach (var key in keys) await _db.KeyDeleteAsync(key);
        return Result.Success();
    }

    public async Task<Result> ResetMonthlyLimitsAsync(CancellationToken cancellationToken = default)
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: $"{KEY_PREFIX}monthly:*").ToArray();
        foreach (var key in keys) await _db.KeyDeleteAsync(key);
        return Result.Success();
    }

    // ═══════════════════════════════════════
    // LIMIT RESOLUTION — Öncelik: CARD > BIN > DEFAULT
    // ═══════════════════════════════════════

    private async Task<(decimal daily, decimal monthly)> ResolveCardLimitsAsync(
        string cardNumber, CancellationToken ct)
    {
        var (daily, monthly, _) = await ResolveCardLimitsWithSingleAsync(cardNumber, ct);
        return (daily, monthly);
    }

    private async Task<(decimal daily, decimal monthly, decimal? single)> ResolveCardLimitsWithSingleAsync(
        string cardNumber, CancellationToken ct)
    {
        try
        {
            // 1. Redis cache kontrol
            var cacheKey = $"{LIMIT_DEF_CACHE_PREFIX}{cardNumber}";
            var cached = await _db.StringGetAsync(cacheKey);
            if (!cached.IsNullOrEmpty)
            {
                var parts = cached.ToString().Split('|');
                if (parts.Length == 3)
                    return (decimal.Parse(parts[0]), decimal.Parse(parts[1]),
                        string.IsNullOrEmpty(parts[2]) ? null : decimal.Parse(parts[2]));
            }

            // 2. CardApplication — kart bazlı limit (asıl kaynak)
            var cardLimit = await _cardLimitProvider.GetCardLimitAsync(cardNumber, ct);
            if (cardLimit != null)
            {
                var value = $"{cardLimit.DailyLimit}|{cardLimit.MonthlyLimit}|";
                await _db.StringSetAsync(cacheKey, value, LimitDefCacheTtl);
                return (cardLimit.DailyLimit, cardLimit.MonthlyLimit, null);
            }

            // 3. CardLimitDefinition — kart bazlı override
            var cardOverride = await _limitRepo.GetByCardNoAsync(cardNumber, ct);
            if (cardOverride != null)
            {
                await CacheLimitDef(cacheKey, cardOverride);
                return (cardOverride.DailyLimit, cardOverride.MonthlyLimit, cardOverride.SingleTransactionLimit);
            }

            // 4. CardLimitDefinition — BIN bazlı
            var bin = cardNumber.Replace("-", "").Replace("X", "").Replace("x", "");
            if (bin.Length >= 6)
            {
                bin = bin[..6];
                var binLimit = await _limitRepo.GetByBinAsync(bin, ct);
                if (binLimit != null)
                {
                    await CacheLimitDef(cacheKey, binLimit);
                    return (binLimit.DailyLimit, binLimit.MonthlyLimit, binLimit.SingleTransactionLimit);
                }
            }

            // 5. CardLimitDefinition — global default
            var defaultLimit = await _limitRepo.GetDefaultAsync(ct);
            if (defaultLimit != null)
            {
                await CacheLimitDef(cacheKey, defaultLimit);
                return (defaultLimit.DailyLimit, defaultLimit.MonthlyLimit, defaultLimit.SingleTransactionLimit);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Limit çözümleme hatası, fallback kullanılıyor: Card={Card}", cardNumber);
        }

        return (FALLBACK_DAILY_LIMIT, FALLBACK_MONTHLY_LIMIT, null);
    }

    private async Task CacheLimitDef(string cacheKey, CardLimitDefinition def)
    {
        var value = $"{def.DailyLimit}|{def.MonthlyLimit}|{def.SingleTransactionLimit}";
        await _db.StringSetAsync(cacheKey, value, LimitDefCacheTtl);
    }

    /// <summary>
    /// Limit tanımı güncellendiğinde cache'i temizle.
    /// Controller'dan çağrılır.
    /// </summary>
    public async Task InvalidateLimitCacheAsync(string? cardNumber = null)
    {
        if (cardNumber != null)
        {
            await _db.KeyDeleteAsync($"{LIMIT_DEF_CACHE_PREFIX}{cardNumber}");
        }
        else
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: $"{LIMIT_DEF_CACHE_PREFIX}*").ToArray();
            foreach (var key in keys) await _db.KeyDeleteAsync(key);
        }
    }

    // ═══════════════════════════════════════
    // HELPERS
    // ═══════════════════════════════════════

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