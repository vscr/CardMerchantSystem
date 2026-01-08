using Merchant.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Merchant.Application.Commands;

public record AddTerminalCommand(
    Guid MerchantId,
    AddTerminalDto Dto
) : IRequest<Result<TerminalDto>>;