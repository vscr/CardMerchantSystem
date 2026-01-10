using HSM.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace HSM.Application.Commands;

public record GeneratePINBlockCommand(GeneratePINBlockRequestDto Request) : IRequest<Result<GeneratePINBlockResponseDto>>;