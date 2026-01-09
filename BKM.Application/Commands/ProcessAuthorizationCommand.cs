using BKM.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace BKM.Application.Commands;

public record ProcessAuthorizationCommand(AuthorizationRequestDto Request) : IRequest<Result<AuthorizationResponseDto>>;