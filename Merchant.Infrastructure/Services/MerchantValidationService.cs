using CardMerchantSystem.Shared.Kernel;
using CardMerchantSystem.Shared.Services;
using Merchant.Domain.Repositories;

namespace Merchant.Infrastructure.Services;

public class MerchantValidationService : IMerchantValidationService
{
    private readonly IMerchantRepository _merchantRepo;

    public MerchantValidationService(IMerchantRepository merchantRepo)
    {
        _merchantRepo = merchantRepo;
    }

    public async Task<Result> ValidateAsync(Guid merchantId, Guid terminalId, CancellationToken ct = default)
    {
        var merchant = await _merchantRepo.GetByIdAsync(merchantId, ct);
        if (merchant == null)
            return Result.Failure("Üye işyeri bulunamadı", ErrorCodes.NotFound);

        if (!merchant.Status.CanProcessTransactions)
            return Result.Failure($"Üye işyeri aktif değil. Durum: {merchant.Status.DisplayName}", ErrorCodes.ValidationError);

        var terminal = merchant.Terminals?.FirstOrDefault(t => t.Id == terminalId);
        if (terminal == null)
            return Result.Failure("Terminal bulunamadı", ErrorCodes.NotFound);

        if (!terminal.Status.CanProcessTransactions)
            return Result.Failure($"Terminal aktif değil. Durum: {terminal.Status.DisplayName}", ErrorCodes.ValidationError);

        return Result.Success();
    }
}