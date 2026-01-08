using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Merchant.Application.Commands;

public record ActivateMerchantCommand(
    Guid MerchantId,
    string OperatorUsername
) : IRequest<Result>;