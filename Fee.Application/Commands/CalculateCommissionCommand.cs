using Fee.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Fee.Application.Commands;

public record CalculateCommissionCommand(CalculateCommissionRequestDto Request) : IRequest<Result<CalculateCommissionResponseDto>>;