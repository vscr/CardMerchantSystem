using Fee.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Fee.Application.Commands;

public record AssignMerchantTariffCommand(AssignMerchantTariffDto Dto) : IRequest<Result<MerchantTariffDto>>;