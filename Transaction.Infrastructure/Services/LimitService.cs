using Transaction.Domain.Services;
using Transaction.Domain.ValueObjects;
using CardMerchantSystem.Shared.Kernel;
using StackExchange.Redis;

namespace Transaction.Infrastructure.Services;

/// <summary>
/// Redis tabanlı Limit Kontrol Sistemi (LKS)
/// </summary>
public class LimitService : ILimitService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    // Default limitler
    private const decimal DEFAULT_DAILY_LIMIT = 10000m;
    private const decimal DEFAULT_MONTHLY_LIMIT = 50000m;
    private const string KEY_PREFIX = "card_limit:";

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

    public async Task<Result> CheckLimitAsync(string cardNumber, decimal amount, CancellationToken cancellationToken = default)
    {
        var limitResult = await GetCardLimitAsync(cardNumber, cancellationToken);
        if (limitResult.IsFailure)
            return Result.Failure(limitResult.Error!, limitResult.ErrorCode);

        var limit = limitResult.Value!;

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

        // Rezervasyonu kaydet
        await _db.StringSetAsync(reserveKey, amount.ToString(), TimeSpan.FromMinutes(30));

        return Result.Success();
    }

    public async Task<Result> CommitLimitAsync(string cardNumber, decimal amount, string transactionId, CancellationToken cancellationToken = default)
    {
        var dailyKey = GetDailyKey(cardNumber);
        var monthlyKey = GetMonthlyKey(cardNumber);
        var reserveKey = GetReserveKey(cardNumber, transactionId);

        // Günlük kullanımı artır
        await _db.StringIncrementAsync(dailyKey, (long)(amount * 100));
        await _db.KeyExpireAsync(dailyKey, GetEndOfDay());

        // Aylık kullanımı artır
        await _db.StringIncrementAsync(monthlyKey, (long)(amount * 100));
        await _db.KeyExpireAsync(monthlyKey, GetEndOfMonth());

        // Rezervasyonu sil
        await _db.KeyDeleteAsync(reserveKey);

        return Result.Success();
    }

    public async Task<Result> ReleaseLimitAsync(string cardNumber, decimal amount, string transactionId, CancellationToken cancellationToken = default)
    {
        var reserveKey = GetReserveKey(cardNumber, transactionId);

        // Sadece rezervasyonu sil
        await _db.KeyDeleteAsync(reserveKey);

        return Result.Success();
    }

    public async Task<Result> RefundLimitAsync(string cardNumber, decimal amount, string transactionId, CancellationToken cancellationToken = default)
    {
        var dailyKey = GetDailyKey(cardNumber);
        var monthlyKey = GetMonthlyKey(cardNumber);

        // Günlük kullanımı azalt
        var dailyUsed = await GetDecimalValue(dailyKey);
        var newDailyUsed = Math.Max(0, dailyUsed - amount);
        await _db.StringSetAsync(dailyKey, ((long)(newDailyUsed * 100)).ToString(), GetEndOfDay());

        // Aylık kullanımı azalt
        var monthlyUsed = await GetDecimalValue(monthlyKey);
        var newMonthlyUsed = Math.Max(0, monthlyUsed - amount);
        await _db.StringSetAsync(monthlyKey, ((long)(newMonthlyUsed * 100)).ToString(), GetEndOfMonth());

        return Result.Success();
    }

    public async Task<Result> ResetDailyLimitsAsync(CancellationToken cancellationToken = default)
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: $"{KEY_PREFIX}daily:*").ToArray();

        foreach (var key in keys)
        {
            await _db.KeyDeleteAsync(key);
        }

        return Result.Success();
    }

    public async Task<Result> ResetMonthlyLimitsAsync(CancellationToken cancellationToken = default)
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: $"{KEY_PREFIX}monthly:*").ToArray();

        foreach (var key in keys)
        {
            await _db.KeyDeleteAsync(key);
        }

        return Result.Success();
    }

    private static string GetDailyKey(string cardNumber) =>
        $"{KEY_PREFIX}daily:{cardNumber}:{DateTime.UtcNow:yyyyMMdd}";

    private static string GetMonthlyKey(string cardNumber) =>
        $"{KEY_PREFIX}monthly:{cardNumber}:{DateTime.UtcNow:yyyyMM}";

    private static string GetReserveKey(string cardNumber, string transactionId) =>
        $"{KEY_PREFIX}reserve:{cardNumber}:{transactionId}";

    private async Task<decimal> GetDecimalValue(string key)
    {
        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty)
            return 0m;

        if (long.TryParse(value, out var longValue))
            return longValue / 100m;

        return 0m;
    }

    private static TimeSpan GetEndOfDay()
    {
        var now = DateTime.UtcNow;
        var endOfDay = now.Date.AddDays(1);
        return endOfDay - now;
    }

    private static TimeSpan GetEndOfMonth()
    {
        var now = DateTime.UtcNow;
        var endOfMonth = new DateTime(now.Year, now.Month, 1).AddMonths(1);
        return endOfMonth - now;
    }
}