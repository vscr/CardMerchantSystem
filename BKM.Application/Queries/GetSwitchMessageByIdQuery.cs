using BKM.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace BKM.Application.Queries;

public record GetSwitchMessageByIdQuery(Guid Id) : IRequest<Result<SwitchMessageDto>>;