using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Merchant.Application.Commands;

public record ApproveMerchantCommand(
    Guid MerchantId,
    string ApproverUsername
) : IRequest<Result>;