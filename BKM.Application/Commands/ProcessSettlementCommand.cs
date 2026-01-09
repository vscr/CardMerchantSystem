using BKM.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace BKM.Application.Commands;

public record ProcessSettlementCommand(string SettlementDate) : IRequest<Result<SettlementBatchDto>>;