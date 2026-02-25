using CardMerchantSystem.Shared.Services;
using EarlyBlockResolution.Domain.Repositories;

namespace EarlyBlockResolution.Infrastructure.Services;

public class CardBlockCheckService : ICardBlockCheckService
{
    private readonly ICardBlockRepository _blockRepo;

    public CardBlockCheckService(ICardBlockRepository blockRepo)
    {
        _blockRepo = blockRepo;
    }

    public async Task<bool> IsBlockedAsync(string cardNumberMasked, CancellationToken ct = default)
    {
        var activeBlocks = await _blockRepo.GetActiveBlocksByCardMaskedAsync(cardNumberMasked, ct);
        return activeBlocks.Any();
    }
}