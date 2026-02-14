using Card.Domain.Repositories;
using CardMerchantSystem.Shared.Services;
using Microsoft.Extensions.Logging;

namespace Card.Infrastructure.Services;

/// <summary>
/// CardApplication tablosundan kart limiti okur.
/// Kart başvurusu sırasında belirlenen DailyLimit/MonthlyLimit döner.
/// </summary>
public class CardLimitProvider : ICardLimitProvider
{
    private readonly ICardApplicationRepository _cardRepo;
    private readonly ILogger<CardLimitProvider> _logger;

    public CardLimitProvider(ICardApplicationRepository cardRepo, ILogger<CardLimitProvider> logger)
    {
        _cardRepo = cardRepo;
        _logger = logger;
    }

    public async Task<CardLimitInfo?> GetCardLimitAsync(string maskedCardNo, CancellationToken ct = default)
    {
        try
        {
            var card = await _cardRepo.GetByMaskedCardNoAsync(maskedCardNo, ct);
            if (card == null) return null;

            return new CardLimitInfo
            {
                MaskedCardNo = maskedCardNo,
                DailyLimit = card.DailyLimit.Amount,
                MonthlyLimit = card.MonthlyLimit.Amount,
                Currency = card.DailyLimit.Currency
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kart limiti okunamadı: {CardNo}", maskedCardNo);
            return null;
        }
    }
}