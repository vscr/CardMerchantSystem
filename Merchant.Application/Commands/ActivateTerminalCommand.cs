using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Merchant.Application.Commands;

public record ActivateTerminalCommand(
    Guid MerchantId,
    Guid TerminalId,
    string OperatorUsername
) : IRequest<Result>;