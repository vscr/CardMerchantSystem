using CardMerchantSystem.Shared.Kernel;

namespace CardMerchantSystem.Shared.Services;

public interface IMerchantValidationService
{
    Task<Result> ValidateAsync(Guid merchantId, Guid terminalId, CancellationToken ct = default);
}