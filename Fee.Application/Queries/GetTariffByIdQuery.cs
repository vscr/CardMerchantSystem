using Fee.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Fee.Application.Queries;

public record GetTariffByIdQuery(Guid Id) : IRequest<Result<TariffDto>>;