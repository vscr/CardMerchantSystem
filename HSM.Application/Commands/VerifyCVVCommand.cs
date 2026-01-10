using HSM.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace HSM.Application.Commands;

public record VerifyCVVCommand(VerifyCVVRequestDto Request) : IRequest<Result<VerifyCVVResponseDto>>;