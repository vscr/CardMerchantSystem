using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Dispute.Application.Commands;

public record ResolveDisputeCommand(
    Guid DisputeId,
    bool InFavorOfCustomer,
    decimal? RefundAmount,
    string Resolution,
    string OperatorUsername
) : IRequest<Result>;