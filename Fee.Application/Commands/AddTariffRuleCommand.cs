using Fee.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Fee.Application.Commands;

public record AddTariffRuleCommand(AddTariffRuleDto Dto) : IRequest<Result<TariffDto>>;