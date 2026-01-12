using Fee.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Fee.Application.Commands;

public record ActivateTariffCommand(Guid TariffId) : IRequest<Result<TariffDto>>;