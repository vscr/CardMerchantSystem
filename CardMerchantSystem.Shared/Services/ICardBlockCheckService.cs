namespace CardMerchantSystem.Shared.Services;

public interface ICardBlockCheckService
{
    Task<bool> IsBlockedAsync(string cardNumberMasked, CancellationToken ct = default);
}