using CardMerchantSystem.Shared.Idempotency;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Transaction.Application.Commands;
using Transaction.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;

namespace Transaction.Application.Behaviors;

/// <summary>
/// MediatR Pipeline Behavior — ProcessTransactionCommand için idempotency.
/// 
/// Pipeline sırası:
/// Request → ValidationBehavior → IdempotencyBehavior → Handler → Response
/// 
/// Bu behavior sadece ProcessTransactionCommand için çalışır.
/// Generic yapılabilir ama bankacılıkta sadece finansal işlemler
/// idempotent olmalı (sorgu işlemleri hariç).
/// 
/// Akış:
/// ─────
/// 1. Idempotency-Key header'dan gelir (middleware tarafından enjekte edilir)
/// 2. Key ile önceki kayıt aranır
/// 3. Kayıt varsa → cache'den response döner (işlem yapılmaz)
/// 4. Kayıt yoksa → lock alınır → handler çalışır → sonuç kaydedilir
/// </summary>
public class IdempotencyBehavior
    : IPipelineBehavior<ProcessTransactionCommand, Result<TransactionResultDto>>
{
    private readonly IIdempotencyStore _store;
    private readonly IIdempotencyContext _context;
    private readonly ILogger<IdempotencyBehavior> _logger;

    public IdempotencyBehavior(
        IIdempotencyStore store,
        IIdempotencyContext context,
        ILogger<IdempotencyBehavior> logger)
    {
        _store = store;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<TransactionResultDto>> Handle(
        ProcessTransactionCommand request,
        RequestHandlerDelegate<Result<TransactionResultDto>> next,
        CancellationToken cancellationToken)
    {
        // Idempotency key yoksa → normal akış (backward compatible)
        var idempotencyKey = _context.IdempotencyKey;
        if (string.IsNullOrEmpty(idempotencyKey))
            return await next();

        // Request body hash (aynı key + farklı body = hata)
        var requestHash = ComputeHash(request.Dto);

        // ═══ STEP 1: Önceki kayıt var mı? ═══
        var existing = await _store.GetAsync(idempotencyKey, cancellationToken);

        if (existing != null)
        {
            // Hash kontrolü — aynı key ile farklı body gönderildi mi?
            if (existing.RequestHash != requestHash)
            {
                _logger.LogWarning(
                    "Idempotency hash mismatch! Key={Key} — olası hata veya saldırı",
                    idempotencyKey);

                return Result.Failure<TransactionResultDto>(
                    "Idempotency-Key daha önce farklı bir istek ile kullanıldı",
                    "IDEMPOTENCY_HASH_MISMATCH");
            }

            // Processing durumunda → başka bir istek şu an işliyor
            if (existing.Status == "Processing")
            {
                _logger.LogInformation(
                    "Idempotency concurrent request: Key={Key}", idempotencyKey);

                return Result.Failure<TransactionResultDto>(
                    "Bu işlem şu an işleniyor, lütfen bekleyiniz",
                    "IDEMPOTENCY_PROCESSING");
            }

            // Completed → cache'den döndür
            if (existing.Status == "Completed" && existing.ResponseBody != null)
            {
                await _store.IncrementRetryAsync(idempotencyKey, cancellationToken);

                var cachedResult = JsonSerializer.Deserialize<TransactionResultDto>(existing.ResponseBody);

                _logger.LogInformation(
                    "Idempotency cache hit: Key={Key}, TxId={TxId}, RetryCount={Retry}",
                    idempotencyKey, existing.TransactionId, existing.RetryCount + 1);

                // Orijinal sonucu aynen döndür
                if (cachedResult!.IsApproved)
                    return Result.Success(cachedResult);
                else
                    return Result.Failure<TransactionResultDto>(
                        cachedResult.ResponseMessage, cachedResult.ResponseCode);
            }
        }

        // ═══ STEP 2: Lock al (Redis SET NX) ═══
        var record = new IdempotencyRecord
        {
            IdempotencyKey = idempotencyKey,
            RequestPath = "/api/Transactions/process",
            RequestHash = requestHash,
            Status = "Processing",
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        var lockAcquired = await _store.TryCreateAsync(record, cancellationToken);
        if (!lockAcquired)
        {
            // Başka instance lock aldı — concurrent
            return Result.Failure<TransactionResultDto>(
                "Bu işlem başka bir sunucu tarafından işleniyor",
                "IDEMPOTENCY_LOCKED");
        }

        // ═══ STEP 3: İşlemi çalıştır ═══
        try
        {
            var result = await next();

            // Sonucu kaydet
            var resultDto = result.IsSuccess ? result.Value! : new TransactionResultDto
            {
                IsApproved = false,
                ResponseCode = result.ErrorCode ?? "99",
                ResponseMessage = result.Error ?? "Bilinmeyen hata",
                Amount = request.Dto.Amount,
                Currency = request.Dto.Currency,
                TransactionTime = DateTime.UtcNow
            };

            var responseJson = JsonSerializer.Serialize(resultDto);
            var statusCode = result.IsSuccess ? 200 : 400;

            await _store.CompleteAsync(
                idempotencyKey, statusCode, responseJson,
                resultDto.Id != Guid.Empty ? resultDto.Id : null,
                cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            // Transient hata → lock'u serbest bırak
            _logger.LogError(ex, "Idempotent işlem hatası: Key={Key}", idempotencyKey);

            await _store.FailAsync(idempotencyKey, 500,
                JsonSerializer.Serialize(new { error = ex.Message }),
                cancellationToken);

            throw;
        }
    }

    /// <summary>
    /// Request body'nin SHA256 hash'i.
    /// Deterministic olmalı — aynı input her zaman aynı hash.
    /// </summary>
    private static string ComputeHash(CreateTransactionDto dto)
    {
        var content = $"{dto.TransactionTypeId}|{dto.Amount}|{dto.Currency}|" +
                      $"{dto.CardNumberMasked}|{dto.MerchantCode}|{dto.TerminalCode}|" +
                      $"{dto.OriginalTransactionId}";

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(content));
        return Convert.ToHexString(bytes);
    }
}