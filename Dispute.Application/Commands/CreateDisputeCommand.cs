using Dispute.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Dispute.Application.Commands;

public record CreateDisputeCommand(CreateDisputeDto Dto) : IRequest<Result<DisputeDto>>;