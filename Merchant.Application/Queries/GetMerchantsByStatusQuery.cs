using Merchant.Application.DTOs;
using MediatR;

namespace Merchant.Application.Queries;

public record GetMerchantsByStatusQuery(int StatusId) : IRequest<IReadOnlyList<MerchantDto>>;