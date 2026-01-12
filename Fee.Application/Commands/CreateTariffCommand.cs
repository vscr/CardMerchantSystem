using Fee.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Fee.Application.Commands;

public record CreateTariffCommand(CreateTariffDto Dto) : IRequest<Result<TariffDto>>;