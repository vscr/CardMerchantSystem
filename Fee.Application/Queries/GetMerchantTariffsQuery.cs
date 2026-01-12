using Fee.Application.DTOs;
using MediatR;

namespace Fee.Application.Queries;

public record GetMerchantTariffsQuery(string MerchantId) : IRequest<IReadOnlyList<MerchantTariffDto>>;