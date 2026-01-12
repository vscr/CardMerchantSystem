using Fee.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Fee.Application.Commands;

public record CreateFeeAccrualCommand(CreateFeeAccrualDto Dto) : IRequest<Result<FeeAccrualDto>>;